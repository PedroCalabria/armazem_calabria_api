using ArmazemCalabria.Entity.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmazemCalabria.Business.IBusiness
{
    public interface IUsuarioBusiness
    {
        public Task AutorizarPerfilGestor(string emailUsuario);
        public Task<List<UsuarioDTO>> ConsultarUsuariosPendentesAprovacao();
    }
}
