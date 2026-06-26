# Pedidos — setup do banco de dados

## 1. Executar o DDL

Após garantir que as tabelas de usuários e pisos já existem, execute:

```
armazem_calabria/ArmazemCalabria.Utils/Documents/DDL_Pedidos.sql
```

No SQL Server Management Studio ou `sqlcmd`, conectado ao banco `armazem_calabria_db`.

O script cria:

- `armazem.tb_pedidos` — cabeçalho do pedido (solicitante, status, datas, aprovador/rejeição)
- `armazem.tb_pedidos_itens` — itens do pedido (piso + quantidade)

## 2. Validar perfis em `usuario.tb_perfis`

As descrições devem coincidir com [`Constants.cs`](../Constants/Constants.cs) para autorização JWT:

| id_perfil | descricao           |
|-----------|---------------------|
| 1         | Gestor              |
| 2         | Lojista Interno     |
| 3         | Lojista Externo     |

Consulta de verificação:

```sql
SELECT id_perfil, descricao FROM usuario.tb_perfis ORDER BY id_perfil;
```

Se necessário, corrija:

```sql
UPDATE usuario.tb_perfis SET descricao = 'Lojista Interno' WHERE id_perfil = 2;
UPDATE usuario.tb_perfis SET descricao = 'Lojista Externo' WHERE id_perfil = 3;
```

## 3. Reiniciar a API

Após executar o DDL, reinicie a API .NET para carregar os novos mappings EF Core.

## 4. Teste rápido

1. Login como **Lojista Externo** → `/pedidos` → "Solicitar novo pedido"
2. Login como **Gestor** ou **Lojista Interno** → aprovar/rejeitar pendentes na grid
