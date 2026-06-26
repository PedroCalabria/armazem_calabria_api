using ArmazemCalabria.Business.IBusiness;
using ArmazemCalabria.CrossCutting;
using ArmazemCalabria.CrossCutting.Exceptions;
using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Entities;
using ArmazemCalabria.Entity.Enum;
using ArmazemCalabria.Repository.IRepository;
using ArmazemCalabria.Utils.Extensions;
using Microsoft.AspNetCore.Http;

namespace ArmazemCalabria.Business.Imp.Business
{
    public class PedidoBusiness(
        IPedidoRepository _repository,
        IUserContext _userContext,
        IHttpContextAccessor _httpContextAccessor) : IPedidoBusiness
    {
        public async Task<List<PedidoGridItemDTO>> ConsultarPedidos()
        {
            var idPerfil = ObterIdPerfil();
            var idUsuario = ObterIdUsuario();

            int? filtroSolicitante = idPerfil == TiposPerfilUsuario.LogistaExterno.AsInt()
                ? idUsuario
                : null;

            var pedidos = await _repository.ConsultarPedidos(filtroSolicitante);
            var podeGerenciar = PodeGerenciarPedidos(idPerfil);

            return pedidos.Select(p => MapearParaGrid(p, podeGerenciar)).ToList();
        }

        public async Task<PedidoCriadoDTO> SolicitarPedido(SolicitarPedidoDTO dto)
        {
            var idPerfil = ObterIdPerfil();
            var idUsuario = ObterIdUsuario();

            if (idPerfil == TiposPerfilUsuario.Gestor.AsInt())
                throw new BusinessException("Gestores não podem solicitar pedidos.");

            if (idPerfil != TiposPerfilUsuario.LogistaExterno.AsInt())
                throw new BusinessException("Você não tem permissão para esta operação.");

            var itensAgrupados = ValidarEAgregarItens(dto.Itens);
            var dadosPisos = await _repository.ObterDadosPisos(itensAgrupados.Keys);

            ValidarPisosExistem(itensAgrupados.Keys, dadosPisos);

            var estoqueSuficiente = itensAgrupados.All(item =>
                dadosPisos[item.Key].QuantidadeDisponivel >= item.Value);

            var status = estoqueSuficiente ? StatusPedido.Aprovado : StatusPedido.Pendente;
            var dataAtual = DateTime.UtcNow;

            var pedido = new Pedido
            {
                IdUsuarioSolicitante = idUsuario,
                IdStatus = status,
                DataCriacao = dataAtual,
                Itens = itensAgrupados.Select(item => new PedidoItem
                {
                    IdPiso = item.Key,
                    Quantidade = item.Value
                }).ToList()
            };

            if (status == StatusPedido.Aprovado)
            {
                pedido.DataAprovacao = dataAtual;
            }

            await _repository.InsertAsync(pedido);

            if (status == StatusPedido.Aprovado)
                await BaixarEstoquePedido(pedido.Itens, dadosPisos);

            return new PedidoCriadoDTO
            {
                IdPedido = pedido.IdPedido,
                IdStatus = status.AsInt(),
                Status = status.GetDescription()
            };
        }

        public async Task AprovarPedido(int idPedido)
        {
            ValidarPodeGerenciarPedidos();

            var idUsuario = ObterIdUsuario();
            var pedido = await ObterPedidoPendente(idPedido);
            var dadosPisos = await _repository.ObterDadosPisos(pedido.Itens.Select(i => i.IdPiso));

            ValidarEstoqueSuficiente(pedido.Itens, dadosPisos);
            await BaixarEstoquePedido(pedido.Itens, dadosPisos);

            pedido.IdStatus = StatusPedido.Aprovado;
            pedido.IdUsuarioAprovador = idUsuario;
            pedido.DataAprovacao = DateTime.UtcNow;
            pedido.DataAlteracao = DateTime.UtcNow;

            await _repository.UpdateAsync(pedido);
        }

        public async Task<AprovarPedidosResultadoDTO> AprovarPedidos(AprovarPedidosDTO dto)
        {
            ValidarPodeGerenciarPedidos();

            if (dto.IdsPedidos is null || dto.IdsPedidos.Count == 0)
                throw new BusinessException("Informe ao menos um pedido para aprovação.");

            var resultado = new AprovarPedidosResultadoDTO();

            foreach (var idPedido in dto.IdsPedidos.Distinct())
            {
                try
                {
                    await AprovarPedido(idPedido);
                    resultado.Aprovados.Add(idPedido);
                }
                catch (BusinessException ex)
                {
                    resultado.Falhas.Add(new AprovarPedidoFalhaDTO
                    {
                        IdPedido = idPedido,
                        Motivo = ex.Message
                    });
                }
                catch (InvalidOperationException ex)
                {
                    resultado.Falhas.Add(new AprovarPedidoFalhaDTO
                    {
                        IdPedido = idPedido,
                        Motivo = ex.Message
                    });
                }
            }

            return resultado;
        }

        public async Task RejeitarPedido(RejeitarPedidoDTO dto)
        {
            ValidarPodeGerenciarPedidos();

            var idUsuario = ObterIdUsuario();
            var pedido = await ObterPedidoPendente(dto.IdPedido);

            pedido.IdStatus = StatusPedido.Rejeitado;
            pedido.MotivoRejeicao = dto.MotivoRejeicao;
            pedido.IdUsuarioAprovador = idUsuario;
            pedido.DataRejeicao = DateTime.UtcNow;
            pedido.DataAlteracao = DateTime.UtcNow;

            await _repository.UpdateAsync(pedido);
        }

        #region private methods

        private int ObterIdUsuario()
        {
            var userId = _userContext.AdditionalData["UserId"]?.ToString();

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var id))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return id;
        }

        private int ObterIdPerfil()
        {
            var idPerfil = _userContext.AdditionalData["IdPerfil"]?.ToString();

            if (string.IsNullOrEmpty(idPerfil))
                idPerfil = _httpContextAccessor.HttpContext?.User.FindFirst("idRole")?.Value;

            if (string.IsNullOrEmpty(idPerfil) || !int.TryParse(idPerfil, out var id))
                throw new UnauthorizedAccessException("Perfil do usuário não identificado.");

            return id;
        }

        private static bool PodeGerenciarPedidos(int idPerfil)
        {
            return idPerfil == TiposPerfilUsuario.Gestor.AsInt()
                || idPerfil == TiposPerfilUsuario.LogistaInterno.AsInt();
        }

        private void ValidarPodeGerenciarPedidos()
        {
            if (!PodeGerenciarPedidos(ObterIdPerfil()))
                throw new BusinessException("Você não tem permissão para esta operação.");
        }

        private static Dictionary<int, int> ValidarEAgregarItens(List<PedidoItemSolicitacaoDTO> itens)
        {
            if (itens is null || itens.Count == 0)
                throw new BusinessException("Informe ao menos um item no pedido.");

            var agrupados = new Dictionary<int, int>();

            foreach (var item in itens)
            {
                if (item.IdPiso <= 0)
                    throw new BusinessException("Piso inválido.");

                if (item.Quantidade <= 0)
                    throw new BusinessException("Quantidade deve ser maior que zero.");

                agrupados[item.IdPiso] = agrupados.GetValueOrDefault(item.IdPiso) + item.Quantidade;
            }

            return agrupados;
        }

        private static void ValidarPisosExistem(
            IEnumerable<int> idsPiso,
            Dictionary<int, (string Nome, int QuantidadeDisponivel)> dadosPisos)
        {
            foreach (var idPiso in idsPiso)
            {
                if (!dadosPisos.ContainsKey(idPiso))
                    throw new BusinessException("Piso não encontrado.");
            }
        }

        private static void ValidarEstoqueSuficiente(
            IEnumerable<PedidoItem> itens,
            Dictionary<int, (string Nome, int QuantidadeDisponivel)> dadosPisos)
        {
            foreach (var item in itens)
            {
                if (!dadosPisos.TryGetValue(item.IdPiso, out var dados))
                    throw new BusinessException("Piso não encontrado.");

                if (dados.QuantidadeDisponivel < item.Quantidade)
                    throw new BusinessException($"Estoque insuficiente para o piso {dados.Nome}.");
            }
        }

        private async Task BaixarEstoquePedido(
            IEnumerable<PedidoItem> itens,
            Dictionary<int, (string Nome, int QuantidadeDisponivel)> dadosPisos)
        {
            foreach (var item in itens)
            {
                var baixou = await _repository.BaixarEstoque(item.IdPiso, item.Quantidade);

                if (!baixou)
                {
                    var nome = dadosPisos.TryGetValue(item.IdPiso, out var dados)
                        ? dados.Nome
                        : item.IdPiso.ToString();

                    throw new BusinessException($"Estoque insuficiente para o piso {nome}.");
                }
            }
        }

        private async Task<Pedido> ObterPedidoPendente(int idPedido)
        {
            var pedido = await _repository.ObterPedidoComItensParaAtualizacao(idPedido);

            if (pedido is null)
                throw new BusinessException("Pedido não encontrado.");

            if (pedido.IdStatus != StatusPedido.Pendente)
                throw new BusinessException("Apenas pedidos pendentes podem ser alterados.");

            return pedido;
        }

        private static PedidoGridItemDTO MapearParaGrid(Pedido pedido, bool podeGerenciar)
        {
            var pendente = pedido.IdStatus == StatusPedido.Pendente;

            return new PedidoGridItemDTO
            {
                IdPedido = pedido.IdPedido,
                IdStatus = pedido.IdStatus.AsInt(),
                Status = pedido.IdStatus.GetDescription(),
                NomeSolicitante = pedido.UsuarioSolicitante?.Nome ?? string.Empty,
                DataCriacao = pedido.DataCriacao,
                MotivoRejeicao = pedido.MotivoRejeicao,
                PodeAprovar = podeGerenciar && pendente,
                PodeRejeitar = podeGerenciar && pendente,
                Itens = pedido.Itens.Select(i => new PedidoItemGridDTO
                {
                    IdPedidoItem = i.IdPedidoItem,
                    IdPiso = i.IdPiso,
                    NomePiso = i.Piso?.Nome ?? string.Empty,
                    Quantidade = i.Quantidade,
                    QuantidadeDisponivel = i.Piso?.QuantidadeDisponivel ?? 0
                }).ToList()
            };
        }

        #endregion
    }
}
