using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Entities;
using ArmazemCalabria.Repository.Imp.Repository.Base;
using ArmazemCalabria.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ArmazemCalabria.Repository.Imp.Repository
{
    public class UsuarioRepository(Context context) : RepositoryBase<Usuario>(context), IUsuarioRepository
    {
        public async Task<Usuario> ObterUsuarioPorEmail(string email)
        {
            return await Context.Usuarios
                .AsNoTracking()
                .Include(u => u.Perfil)
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }
        
        public async Task<Usuario> ObterUsuarioPorId(int id)
        {
            return await Context.Usuarios
                .AsNoTracking()
                .Include(u => u.Perfil)
                .Where(u => u.IdUsuario == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UsuarioDTO>> ConsultarUsuariosPendentesAprovacao()
        {
            return await Context.Usuarios
                .AsNoTracking()
                .Where(u => !u.FlagAprovado)
                .Select(u => new UsuarioDTO
                {
                    IdUsuario = u.IdUsuario,
                    Email = u.Email,
                    Nome = u.Nome,
                    DataCriacao = u.DataCriacao,
                })
                .ToListAsync();
        }
    }
}
