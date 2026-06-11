using ArmazemCalabria.Business.IBusiness;
using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmazemCalabria.Business.Imp.Business
{
    public class UsuarioBusiness(IUsuarioRepository _repository): IUsuarioBusiness
    {
        public async Task AutorizarPerfilGestor(string emailUsuario)
        {
            var usuario = await _repository.ObterUsuarioPorEmail(emailUsuario);

            if (usuario is null)
                throw new InvalidOperationException("Usuário não encontrado.");

            usuario.FlagAprovado = true;
            await _repository.UpdateAsync(usuario);
        }

        public async Task<List<UsuarioDTO>> ConsultarUsuariosPendentesAprovacao()
        {
            var usuariosPendentes = await _repository.ConsultarUsuariosPendentesAprovacao();

            return usuariosPendentes;
        }
    }
}
