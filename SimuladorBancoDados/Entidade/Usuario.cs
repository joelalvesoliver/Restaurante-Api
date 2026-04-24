

namespace SimuladorBancoDados.Entidade
{
    // Entidade Usuario: representa um funcionário/usuário do sistema internamente.
    // Esta classe é "internal" (acesso restrito ao projeto SimuladorBancoDados),
    // ou seja, nenhum outro projeto pode criar ou acessar diretamente um objeto Usuario.
    // Isso protege os dados — para se comunicar com o banco, os outros projetos
    // precisam usar o UsuarioDto (objeto de transferência público).
    internal class Usuario
    {
        // Identificador único gerado pelo banco de dados.
        // "private set" significa que só a própria classe pode alterar este valor —
        // quem está de fora pode apenas LER, nunca modificar diretamente.
        // Isso é o conceito de ENCAPSULAMENTO: proteger os dados internos do objeto.
        public int Id { get; private set; }

        // Nome do usuário. Também com "private set" para proteger a integridade dos dados.
        public string Nome { get; private set; }

        // E-mail do usuário.
        public string Email { get; private set; }

        // Senha do usuário. Em um sistema real, seria armazenada como hash (criptografada).
        public string Senha { get; private set; }

        // Função/cargo do usuário (enum Funcao: Gerente, Garcom, etc.).
        public Funcao Funcao { get; private set; }

        // Indica se o usuário está ativo no sistema.
        public bool Ativo { get; private set; }

        // Data e hora de cadastro do usuário.
        public DateTime DataCadastro { get; private set; }

        // Construtor da classe: é o método chamado quando criamos um novo objeto Usuario.
        // Recebe um UsuarioDto (objeto de transferência) e um Id gerado pelo banco.
        // Assim, garantimos que um Usuario só pode ser criado com todos os dados necessários
        // e que o Id vem sempre de uma fonte confiável (o banco de dados simulado).
        public Usuario(UsuarioDto usuarioDto, int id)
        {
            // Atribui o Id passado como parâmetro
            Id = id;
            // Copia os dados do DTO para as propriedades da entidade
            Nome = usuarioDto.Nome;
            Email = usuarioDto.Email;
            Senha = usuarioDto.Senha;
            Funcao = usuarioDto.Funcao;
            Ativo = usuarioDto.Ativo;
            DataCadastro = usuarioDto.DataCadastro;
        }
    }
}
