using ProjetoLogin.Models;

namespace ProjetoLogin.Repositories
{
    public interface IUsuarioRepositorio
    {
        Task<bool> Cadastrar(UsuarioModel usuario, string senha);

        Task<UsuarioModel> BuscarEmail(string email);

    }
}
