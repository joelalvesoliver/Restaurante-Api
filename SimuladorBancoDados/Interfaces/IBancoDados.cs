
namespace SimuladorBancoDados.Interfaces
{
    public interface IBancoDados
    {
        void AdicionarNovoUsuario(UsuarioDto usuario);
        UsuarioDto? BuscarUsuarioPeloNome(string nome);
        List<UsuarioDto> ListarUsuarios();

        UsuarioDto? BuscarUsuarioPorEmail(string email);
        UsuarioDto? BuscarUsuarioPorNomeEEmail(string nome, string email);

        bool AutenticarUsuario(string email, string senha);
    }
}
