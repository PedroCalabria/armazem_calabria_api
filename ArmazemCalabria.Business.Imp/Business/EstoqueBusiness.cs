using ArmazemCalabria.Business.IBusiness;
using ArmazemCalabria.CrossCutting.Exceptions;
using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Enum;
using ArmazemCalabria.Repository.IRepository;
using ArmazemCalabria.Utils.Extensions;

namespace ArmazemCalabria.Business.Imp.Business
{
    public class EstoqueBusiness(IEstoqueRepository _repository) : IEstoqueBusiness
    {
        public async Task<List<EstoqueGridItemDTO>> ConsultarEstoque(EstoqueFiltroDTO filtro)
        {
            ValidarFiltro(filtro);

            var filtroConsulta = new EstoqueFiltroConsultaDTO
            {
                IdsTipoPiso = ConverterParaIds(filtro.TipoPiso),
                IdsMarca = ConverterParaIds(filtro.Marca),
                IdsNivelResistencia = ConverterParaIds(filtro.NivelResistencia),
                IdsAmbiente = ConverterParaIds(filtro.Ambiente),
                IdsAcabamento = ConverterParaIds(filtro.Acabamento)
            };

            return await _repository.ConsultarEstoque(filtroConsulta);
        }

        public Task<EstoqueOpcoesFiltroDTO> ObterOpcoesFiltro()
        {
            var opcoes = new EstoqueOpcoesFiltroDTO
            {
                TiposPiso = ConverterOpcoes<TiposPiso>(),
                Marcas = ConverterOpcoes<MarcasPiso>(),
                NiveisResistencia = ConverterOpcoes<NiveisResistencia>(),
                Ambientes = ConverterOpcoes<TiposAmbiente>(),
                Acabamentos = ConverterOpcoes<TiposAcabamento>()
            };

            return Task.FromResult(opcoes);
        }

        private static void ValidarFiltro(EstoqueFiltroDTO filtro)
        {
            ValidarLista(filtro.TipoPiso, "Tipo de piso inválido");
            ValidarLista(filtro.Marca, "Marca inválida");
            ValidarLista(filtro.NivelResistencia, "Nível de resistência inválido");
            ValidarLista(filtro.Ambiente, "Ambiente inválido");
            ValidarLista(filtro.Acabamento, "Acabamento inválido");
        }

        private static void ValidarLista<T>(List<T>? valores, string mensagemErro) where T : Enum
        {
            if (valores is null)
                return;

            foreach (var valor in valores)
            {
                if (!valor.IsValid())
                    throw new BusinessException($"{mensagemErro}: {valor}");
            }
        }

        private static List<int> ConverterParaIds<T>(List<T>? valores) where T : Enum
            => valores?.Select(v => v.AsInt()).ToList() ?? [];

        private static List<OpcaoFiltroDTO> ConverterOpcoes<T>() where T : Enum
        {
            return EnumExtensions.ConverterParaLista<T>()
                .Select(o => new OpcaoFiltroDTO
                {
                    Chave = o.Chave,
                    Descricao = o.Descricao
                })
                .ToList();
        }
    }
}
