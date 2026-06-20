using ArmazemCalabria.Entity.DTO;

namespace ArmazemCalabria.Repository.IRepository
{
    public interface IEstoqueRepository
    {
        Task<List<EstoqueGridItemDTO>> ConsultarEstoque(EstoqueFiltroConsultaDTO filtro);
    }
}
