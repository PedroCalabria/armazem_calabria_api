# Importação Assíncrona (Kafka) e Cache (Redis) — Armazém Calábria

Este documento descreve o **funcionamento** e as **decisões de arquitetura** de duas funcionalidades transversais do sistema:

1. **Inclusão de estoque assíncrona via Kafka** (Fases 4 e 5) — upload de planilha `.xlsx`, processamento em background com _bulk insert_ e reprocessamento de pedidos pendentes.
2. **Cache da consulta de estoque com Redis** (Fase 6) — leitura _cache-aside_ com invalidação por chave versionada.

As duas se conectam: toda alteração de estoque produzida pelo fluxo Kafka **invalida** o cache Redis, mantendo a consulta consistente.

---

## Parte 1 — Inclusão de estoque assíncrona com Kafka

### 1.1 Visão geral

O gestor/lojista interno envia uma planilha `.xlsx`. Em vez de processar tudo na requisição HTTP, o sistema:

1. **Persiste** o arquivo no banco (status `Pendente`) e responde imediatamente com um reconhecimento.
2. **Publica** um evento no Kafka apontando para o `IdArquivo`.
3. Um **consumer** em background lê o evento, processa a planilha (leitura + validação + _bulk insert_) e, se entrou estoque, **reprocessa os pedidos pendentes**.
4. O frontend acompanha o andamento via _polling_ de um endpoint de status.

Há ainda um **fallback síncrono** (carga manual) para quando o Kafka/consumer estiver indisponível.

### 1.2 Fluxo detalhado

```
[Cliente] --upload .xlsx--> POST /api/Estoque/importarPlanilha
     |
     v
EnviarPlanilha (ImportacaoEstoqueBusiness)
  1. valida permissão (Gestor / Lojista Interno)
  2. valida arquivo (.xlsx, <= 10 MB, não vazio)
  3. SalvarAsync(arquivo) -> grava conteúdo + status=Pendente  [COMMIT imediato]
  4. PublicarImportacaoAsync(idArquivo) -> Kafka (topic armazem.estoque.importacao)
  5. responde { IdArquivo, Status=Pendente }   <-- resposta rápida ao cliente
     |
     |  (assíncrono)
     v
[Kafka topic] --evento { IdArquivo, DataEnvio }--> ImportacaoConsumerBackgroundService
     |
     v
ProcessarMensagem (por mensagem, em transação própria):
  BeginTransaction(ReadCommitted)
    ProcessarImportacaoAsync(idArquivo):
      - ProcessarAsync: idempotência (só processa se Pendente) -> status=Processando
      - Ler(conteudo) com ClosedXML -> valida layout/cabeçalho
      - constrói Pisos válidos (linha a linha; erros vão para ErroImportacao)
      - ResolverUpsert: separa novos x existentes (soma quantidade nos existentes)
      - PersistirUpsertAsync: AddRange(novos) + SaveChanges  <-- BULK INSERT
      - atualiza status (Processado / ProcessadoComErros / Falha)
      - se (inseridos+atualizados) > 0 -> ReprocessarPedidosPendentes()
  CommitTransaction
  InvalidarAsync() no Redis   <-- invalidação do cache APÓS o commit
  consumer.Commit(offset)     <-- confirma offset só após sucesso (at-least-once)
```

**Polling de status:** `GET /api/Estoque/importacao/{idArquivo}` retorna o status corrente
(`Pendente` → `Processando` → `Processado`/`ProcessadoComErros`/`Falha`) e, em estados terminais
com falha, a lista de erros por linha.

**Fallback de carga manual:** `POST /api/Estoque/processar/{idArquivo}` (restrito ao Gestor) chama o
mesmo `ProcessarImportacaoAsync` de forma **síncrona**, dentro da transação do `ApiMiddleware`. Serve
para quando o Kafka/consumer estiver fora.

### 1.3 Componentes

| Papel | Arquivo | Observação |
|---|---|---|
| Endpoint upload / status / carga manual | `ArmazemCalabria.API/Controllers/EstoqueController.cs` | `importarPlanilha`, `importacao/{id}`, `processar/{id}` |
| Orquestração + leitura + upsert | `ArmazemCalabria.Business.Imp/Business/ImportacaoEstoqueBusiness.cs` | `EnviarPlanilha`, `ProcessarImportacaoAsync`, `ProcessarAsync`, `ResolverUpsert` |
| Producer Kafka | `ArmazemCalabria.Business.Imp/Messaging/KafkaImportacaoEventPublisher.cs` | Singleton; `Acks=All` |
| Consumer Kafka | `ArmazemCalabria.API/Messaging/ImportacaoConsumerBackgroundService.cs` | `BackgroundService`; commit manual de offset |
| Bulk insert / upsert | `ArmazemCalabria.Repository.Imp/Repository/ArquivoImportacaoRepository.cs` | `PersistirUpsertAsync` (`AddRange` + `SaveChanges`) |
| Reprocessamento de pendentes | `ArmazemCalabria.Business.Imp/Business/PedidoBusiness.cs` | `ReprocessarPedidosPendentes` (FIFO) |
| Config Kafka | `ArmazemCalabria.API/Configurations/KafkaConfiguration.cs` + `CrossCutting/Configurations/KafkaSettings.cs` | seção `Kafka` do appsettings |
| Evento | `ArmazemCalabria.Entity/DTO/EventoImportacaoEstoqueDTO.cs` | `{ IdArquivo, DataEnvio }` |

### 1.4 Decisões tomadas

- **Persistir o arquivo e commitar ANTES de publicar no Kafka.** O `importarPlanilha` **não** usa
  `[TransactionRequired]` de propósito: o `SalvarAsync` commita imediatamente. Assim o consumer nunca
  lê um evento cujo registro de arquivo ainda não está commitado (evita _race_ producer/consumer).
- **Evento carrega apenas o `IdArquivo`, não o conteúdo.** A planilha (até 10 MB) fica no banco; o
  evento é leve. O consumer recarrega o arquivo por id. Reduz tráfego no broker e mantém o banco como
  fonte da verdade.
- **`Acks=All` no producer.** Garante durabilidade da publicação (a mensagem só é confirmada após
  replicação), coerente com um evento que dispara escrita de estoque.
- **Commit manual de offset (`EnableAutoCommit=false`), semântica _at-least-once_.** O offset só é
  confirmado após o processamento concluir com sucesso. Se o processo cair no meio, a mensagem é
  reentregue.
- **Idempotência no processamento.** `ProcessarAsync` só processa arquivos em status `Pendente`; em
  redelivery (ou carga manual acidental), arquivos já em estado terminal são ignorados — evita
  duplicar estoque. Esta é a contrapartida obrigatória do _at-least-once_.
- **Uma transação por mensagem, com _rollback_ em caso de falha.** Se qualquer etapa falhar, a
  transação é revertida (inclusive a atualização de status), e o arquivo é marcado como `Falha` em um
  escopo novo. O bulk insert e o reprocessamento de pedidos ficam atômicos por mensagem.
- **Bulk insert via `AddRange` + `SaveChanges` único.** Novos pisos entram em lote; as quantidades de
  pisos já existentes (entidades rastreadas) são atualizadas no mesmo `SaveChanges` — insere e
  atualiza numa só ida ao banco.
- **Consumer roda no mesmo processo da API (`BackgroundService`) e resolve serviços por escopo.** Como
  os _business/repository_ são `Scoped`, cada mensagem cria um `IServiceScope` próprio.
- **Reprocessar pendentes só quando de fato entrou estoque** (`inseridos + atualizados > 0`), em ordem
  **FIFO**, relendo o estoque a cada pedido para respeitar baixas do mesmo lote.

---

## Parte 2 — Cache da consulta de estoque com Redis

### 2.1 Visão geral

A consulta de estoque (`GET /api/Estoque/consultarEstoque`) é **read-through** (_cache-aside_):
serve do Redis quando possível e, em _miss_, consulta o banco e grava o resultado. Como o estoque muda
com baixa frequência relativa às leituras, o cache reduz carga no SQL Server.

A **invalidação** usa a estratégia de **chave versionada**: existe um contador global
`estoque:version`; toda chave de cache embute a versão vigente. Qualquer alteração de estoque faz um
`INCR` nesse contador — todas as chaves da versão anterior ficam órfãs e expiram por TTL, sem precisar
varrer/deletar chaves (`KEYS`/`SCAN`).

### 2.2 Fluxo de leitura (cache-aside)

```
GET /api/Estoque/consultarEstoque?filtros...
     |
     v
EstoqueBusiness.ConsultarEstoque
  - valida filtros (enums) e converte para ids
  - EstoqueCache.ObterOuGravarAsync(filtro, () => repository.ConsultarEstoque(filtro)):
        versao = GET {instance}:estoque:version    (nula => "0")
        chave  = {instance}:estoque:v{versao}:{hashDeterminísticoDoFiltro}
        valor  = GET chave
        se HIT  -> desserializa JSON e retorna (NÃO vai ao banco)
        se MISS -> carrega do banco, SET chave = json (TTL), retorna
```

- **Chave determinística:** cada lista de ids do filtro é **ordenada** antes de compor a chave, então
  filtros equivalentes em ordem diferente (`[1,2]` vs `[2,1]`) batem na mesma entrada.
- **Prefixo `InstanceName`** (`armazem`) isola as chaves da aplicação no Redis.

### 2.3 Fluxo de invalidação (após o commit)

Três gatilhos alteram estoque e devem invalidar o cache. **A invalidação ocorre sempre APÓS o commit**
da transação — assim não há janela em que uma leitura concorrente recacheie dado pré-commit.

**a) Fluxos HTTP — declarativo via atributo**

Endpoints que mexem em estoque recebem `[InvalidatesEstoqueCache]`. O `ApiMiddleware`, depois do
`CommitTransactionAsync` e **somente no caminho de sucesso**, chama `IEstoqueCache.InvalidarAsync()`.
Em exceção/rollback, nada é invalidado.

| Endpoint | Motivo |
|---|---|
| `POST /api/Pedido/solicitarPedido` | pedido pode ser auto-aprovado e baixar estoque |
| `POST /api/Pedido/aprovarPedido` | baixa estoque |
| `POST /api/Pedido/aprovarPedidos` | baixa estoque (lote) |
| `POST /api/Estoque/processar/{id}` | carga manual: insere estoque + reprocessa pendentes |

> Não recebem o atributo: `rejeitarPedido` (não mexe em estoque) e `importarPlanilha` (só salva o
> arquivo e publica no Kafka — a alteração de estoque acontece no consumer).

**b) Fluxo Kafka**

No `ImportacaoConsumerBackgroundService.ProcessarMensagem`, logo após `CommitTransactionAsync`, o
consumer resolve `IEstoqueCache` do escopo e chama `InvalidarAsync()`. Cobre o _bulk insert_ **e** o
reprocessamento de pedidos pendentes que ocorreram na transação da mensagem.

```
Mutação de estoque (HTTP ou Kafka)
   -> COMMIT
      -> INCR {instance}:estoque:version   (v{N} -> v{N+1})
         -> próxima consultarEstoque é MISS -> lê banco -> recacheia sob v{N+1}
```

### 2.4 Componentes

| Papel | Arquivo |
|---|---|
| Contrato do cache | `ArmazemCalabria.Business/ICache/IEstoqueCache.cs` |
| Implementação Redis | `ArmazemCalabria.Business.Imp/Cache/EstoqueCache.cs` |
| Registro DI / conexão | `ArmazemCalabria.API/Configurations/RedisConfiguration.cs` (+ chamada no `Startup.cs`) |
| Configuração | `CrossCutting/Configurations/RedisSettings.cs` (seção `Redis` do appsettings) |
| Atributo marcador | `ArmazemCalabria.Utils/Attributes/InvalidatesEstoqueCacheAttribute.cs` |
| Invalidação HTTP | `ArmazemCalabria.API/Middleware/ApiMiddleware.cs` |
| Invalidação Kafka | `ArmazemCalabria.API/Messaging/ImportacaoConsumerBackgroundService.cs` |
| Read-through | `ArmazemCalabria.Business.Imp/Business/EstoqueBusiness.cs` |

### 2.5 Decisões tomadas

- **Estratégia de invalidação: chave versionada com `INCR`** (em vez de `SCAN`/`DEL` por prefixo ou de
  cachear o dataset inteiro). Motivo: a consulta gera muitas chaves (uma por combinação de filtros); um
  único `INCR` **atômico** aposenta todas as versões antigas de uma vez, sem varredura de chaves.
- **Invalidar APÓS o commit** (não dentro do método de negócio). Motivo: evita a janela em que uma
  leitura concorrente, entre a invalidação e o commit, recacheie um valor ainda não commitado.
- **Invalidação HTTP declarativa por atributo**, espelhando o `[TransactionRequired]` já existente.
  Mantém o `ApiMiddleware` genérico e deixa explícito no controller quais endpoints afetam estoque.
- **Cache na camada de negócio** (`EstoqueBusiness`), não no repositório nem no controller. O
  repositório continua puro (acesso a dados), e o negócio decide política de cache. A `factory`
  passada ao cache é justamente a chamada ao repositório.
- **Biblioteca `StackExchange.Redis` (`IConnectionMultiplexer`)** em vez de `IDistributedCache`. Motivo:
  a estratégia depende de `INCR` atômico (`StringIncrementAsync`), que o `IDistributedCache` não expõe.
- **Degradação graciosa.** A conexão usa `AbortOnConnectFail = false` (a API sobe mesmo sem Redis), e
  as operações de cache são protegidas: se o Redis estiver indisponível, a consulta **cai para o
  banco** (fallback) e a invalidação vira _no-op_, tudo com log — o Redis nunca derruba a requisição.
- **TTL como rede de segurança.** As entradas expiram (`TtlSegundos`, padrão 600s), garantindo que
  chaves órfãs de versões antigas sejam liberadas e limitando a idade máxima de qualquer dado cacheado.
- **Sobre-invalidação aceita como _trade-off_.** Alguns gatilhos podem invalidar sem ter mudado
  estoque de fato (ex.: `solicitarPedido` que fica `Pendente`, ou `processar/{id}` sem inserções). O
  custo é apenas um _cache miss_ extra na próxima leitura — **nunca** dado obsoleto. Optou-se pela
  simplicidade declarativa em vez de um rastreador de "sujeira" mais preciso.

---

## Parte 3 — Infraestrutura

- **`docker-compose.yml`** provê Kafka (KRaft, sem Zookeeper, porta `9092`) e Redis (`redis:7-alpine`,
  porta `6379`, volume `redis-data`). Subir: `docker compose up -d`.
- **`appsettings.json`** — seções relevantes:
  - `Kafka`: `BootstrapServers`, `Topic` (`armazem.estoque.importacao`), `ConsumerGroupId`.
  - `Redis`: `ConnectionString` (`localhost:6379`), `InstanceName` (`armazem`), `TtlSegundos` (`600`).
- **Banco:** SQL Server (`localhost\SQLEXPRESS01`) — fonte da verdade tanto para o estoque quanto para
  os arquivos de importação.

### Como observar em execução

```bash
docker exec armazem-redis redis-cli ping                    # PONG
docker exec armazem-redis redis-cli KEYS "armazem:estoque:*" # chaves de cache + version
docker exec armazem-redis redis-cli GET  "armazem:estoque:version"  # incrementa a cada mutação
```

Sequência esperada: 1ª `consultarEstoque` cria `armazem:estoque:v0:...`; a 2ª (mesmo filtro) é _hit_;
após aprovar pedido / processar planilha, `estoque:version` incrementa e a consulta seguinte é _miss_
com dados atualizados.
