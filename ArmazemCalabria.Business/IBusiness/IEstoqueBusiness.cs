using ArmazemCalabria.Entity.DTO;

namespace ArmazemCalabria.Business.IBusiness
{
    public interface IEstoqueBusiness
    {
        Task<List<EstoqueGridItemDTO>> ConsultarEstoque(EstoqueFiltroDTO filtro);
        Task<EstoqueOpcoesFiltroDTO> ObterOpcoesFiltro();
    }
}
