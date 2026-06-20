using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Entities.Pisos;
using ArmazemCalabria.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ArmazemCalabria.Repository.Imp.Repository
{
    public class EstoqueRepository(Context context) : IEstoqueRepository
    {
        public async Task<List<EstoqueGridItemDTO>> ConsultarEstoque(EstoqueFiltroConsultaDTO filtro)
        {
            var query = context.Pisos.AsNoTracking().AsQueryable();

            if (filtro.IdsTipoPiso.Count > 0)
                query = query.Where(p => filtro.IdsTipoPiso.Contains(p.IdTipoPiso));

            if (filtro.IdsMarca.Count > 0)
                query = query.Where(p => filtro.IdsMarca.Contains(p.IdMarca));

            if (filtro.IdsNivelResistencia.Count > 0)
                query = query.Where(p => filtro.IdsNivelResistencia.Contains(p.IdNivelResistencia));

            if (filtro.IdsAmbiente.Count > 0)
                query = query.Where(p => filtro.IdsAmbiente.Contains(p.IdAmbiente));

            if (filtro.IdsAcabamento.Count > 0)
                query = query.Where(p => filtro.IdsAcabamento.Contains(p.IdAcabamento));

            query = query
                .Include(p => p.TipoPiso)
                .Include(p => p.Marca)
                .Include(p => p.NivelResistencia)
                .Include(p => p.Acabamento)
                .Include(p => p.Ambiente);

            var pisos = await query.ToListAsync();

            return pisos.Select(MapearParaGridItem).ToList();
        }

        private static EstoqueGridItemDTO MapearParaGridItem(Piso p)
        {
            var item = new EstoqueGridItemDTO
            {
                IdPiso = p.IdPiso,
                IdTipoPiso = p.IdTipoPiso,
                Nome = p.Nome,
                Cor = p.Cor,
                Preco = p.Preco,
                QuantidadeDisponivel = p.QuantidadeDisponivel,
                TipoPiso = p.TipoPiso.Descricao,
                Marca = p.Marca.Nome,
                NivelResistencia = p.NivelResistencia.Descricao,
                Ambiente = p.Ambiente.Descricao,
                Acabamento = p.Acabamento.Descricao
            };

            switch (p)
            {
                case PisoCeramica ceramica:
                    item.ClassePei = ceramica.ClassePei;
                    break;
                case PisoPorcelanato porcelanato:
                    item.FlagRetificado = porcelanato.FlagRetificado;
                    item.TipoPorcelanato = porcelanato.TipoPorcelanato;
                    break;
                case PisoVinilico vinilico:
                    item.FlagAcustico = vinilico.FlagAcustico;
                    item.TipoInstalacao = vinilico.TipoInstalacao;
                    break;
                case PisoLaminado laminado:
                    item.FlagResistenteCupim = laminado.FlagResistenteCupim;
                    break;
                case PisoMadeira madeira:
                    item.TipoMadeira = madeira.TipoMadeira;
                    item.FlagMadeiraNobre = madeira.FlagMadeiraNobre;
                    break;
                case PisoPedraNatural pedraNatural:
                    item.TipoPedra = pedraNatural.TipoPedra;
                    item.FlagPorosidade = pedraNatural.FlagPorosidade;
                    item.FlagNecessitaImpermeabilizacao = pedraNatural.FlagNecessitaImpermeabilizacao;
                    break;
            }

            return item;
        }
    }
}
