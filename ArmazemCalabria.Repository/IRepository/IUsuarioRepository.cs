using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Entities;
using ArmazemCalabria.Repository.IRepository.Base;

namespace ArmazemCalabria.Repository.IRepository
{
    public interface IUsuarioRepository : IRepositoryBase<Usuario>
    {
        public Task<Usuario> ObterUsuarioPorEmail(string email);
        public Task<Usuario> ObterUsuarioPorId(int id);
        public Task<List<UsuarioDTO>> ConsultarUsuariosPendentesAprovacao();
    }
}
