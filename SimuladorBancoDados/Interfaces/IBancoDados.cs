
namespace SimuladorBancoDados.Interfaces
{
    // INTERFACE: um "contrato" que define QUAIS métodos uma classe precisa ter,
    // sem definir COMO esses métodos funcionam internamente.
    // Qualquer classe que "implemente" esta interface precisa ter todos esses métodos.
    //
    // Por que usar interfaces?
    // 1. Desacoplamento: o código que usa IBancoDados não precisa saber se o banco
    //    é SQL Server, PostgreSQL, ou um banco em memória (como neste caso).
    // 2. Testabilidade: é fácil criar uma versão "fake" do banco para testes.
    // 3. Flexibilidade: podemos trocar a implementação sem mudar quem usa a interface.
    //
    // Neste projeto, IBancoDados é implementada por BancoDadosService,
    // que simula um banco de dados usando uma lista em memória.
    public interface IBancoDados
    {
        // Adiciona um novo usuário ao banco de dados.
        void AdicionarNovoUsuario(UsuarioDto usuario);

        // Busca um usuário pelo nome. Retorna null se não encontrar.
        // O "?" após UsuarioDto indica que o retorno pode ser nulo ("nullable").
        UsuarioDto? BuscarUsuarioPeloNome(string nome);

        // Retorna uma lista com todos os usuários cadastrados.
        List<UsuarioDto> ListarUsuarios();

        // Busca um usuário pelo e-mail. Retorna null se não encontrar.
        UsuarioDto? BuscarUsuarioPorEmail(string email);

        // Busca um usuário usando tanto o nome quanto o e-mail ao mesmo tempo.
        // Útil para filtros combinados.
        UsuarioDto? BuscarUsuarioPorNomeEEmail(string nome, string email);

        // Verifica se o e-mail e senha são válidos para autenticar (fazer login).
        // Retorna true se as credenciais estão corretas, false se estiverem erradas.
        bool AutenticarUsuario(string email, string senha);
    }
}
