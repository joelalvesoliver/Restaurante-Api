using SimuladorBancoDados.Entidade;
using SimuladorBancoDados.Interfaces;

namespace SimuladorBancoDados.Service
{
    // BancoDadosService implementa a interface IBancoDados.
    // "implementa" significa que esta classe deve ter todos os métodos definidos na interface.
    //
    // Esta classe SIMULA um banco de dados usando uma lista em memória.
    // Em vez de conectar a um banco real (SQL Server, PostgreSQL, etc.),
    // os dados ficam salvos em uma lista enquanto a aplicação está rodando.
    // Quando a aplicação é reiniciada, todos os dados são perdidos.
    // Isso é útil para fins de estudo sem precisar configurar um banco de dados.
    public class BancoDadosService : IBancoDados
    {
        // Lista que armazena todos os usuários em memória.
        // Funciona como se fosse a tabela "Usuarios" de um banco de dados real.
        // "readonly" garante que esta referência nunca será substituída por outra lista.
        private readonly List<Usuario> usuarios;

        // Contador usado para gerar Ids sequenciais para os usuários.
        // Funciona como o AUTO_INCREMENT de um banco de dados relacional.
        // Cada novo usuário recebe o valor atual e o contador é incrementado.
        private int proximoId = 1;

        // Construtor: executado uma única vez quando a aplicação inicia.
        // Inicializa a lista e já adiciona um usuário administrador padrão,
        // para que o sistema não inicie completamente vazio.
        public BancoDadosService()
        {
            // Inicializa a lista vazia de usuários
            usuarios = new List<Usuario>();

            // Cria o usuário administrador padrão usando um UsuarioDto
            var usuario = new UsuarioDto();
            usuario.Nome = "Admin";
            usuario.Email = "admin@admin.com";
            usuario.Ativo = true;
            usuario.DataCadastro = DateTime.Now;
            usuario.Funcao = Funcao.Admin;
            usuario.Senha = "12345";

            // Adiciona o admin na lista, passando o Id atual e incrementando o contador
            usuarios.Add(new Usuario(usuario, proximoId));
            proximoId++;
        }

        // Adiciona um novo usuário na lista (simula um INSERT no banco de dados).
        // Recebe um UsuarioDto (dados públicos) e cria uma entidade Usuario interna.
        public void AdicionarNovoUsuario(UsuarioDto usuarioDto)
        {
            // Cria a entidade interna passando o DTO e o próximo Id disponível
            Usuario novoUsuario = new Usuario(usuarioDto, proximoId);
            // Adiciona na lista
            usuarios.Add(novoUsuario);
            // Incrementa o contador para o próximo usuário
            proximoId++;
        }

        // Verifica se o e-mail e a senha estão corretos (simula autenticação/login).
        // Retorna true se encontrou um usuário com esse e-mail E essa senha.
        // Retorna false se nenhum usuário corresponder.
        public bool AutenticarUsuario(string email, string senha)
        {
            bool autenticado = false;
            // Percorre toda a lista procurando um usuário com e-mail E senha iguais
            foreach (var usuario in usuarios)
            {
                if (usuario.Email == email && usuario.Senha == senha)
                {
                    autenticado = true;
                    break; // Encontrou, pode sair do loop
                }
            }

            return autenticado;
        }

        // Busca um usuário pelo nome (simula um SELECT com WHERE nome = ?).
        // Retorna um UsuarioDto se encontrado, ou null se não existir.
        public UsuarioDto? BuscarUsuarioPeloNome(string nome)
        {
            UsuarioDto usuarioRetorno = null;
            foreach (var usuario in usuarios)
            {
                if(usuario.Nome == nome)
                {
                    // Converte a entidade interna para um DTO antes de retornar.
                    // Isso protege a entidade interna de ser exposta diretamente.
                    usuarioRetorno = new UsuarioDto
                    {
                        Id = usuario.Id,
                        Nome = usuario.Nome,
                        Email = usuario.Email,
                        Ativo = usuario.Ativo,
                        DataCadastro = usuario.DataCadastro,
                        Funcao = usuario.Funcao,
                        Senha = usuario.Senha
                    };

                    break; // Encontrou, pode sair do loop
                }
            }

            return usuarioRetorno;
        }

        // Busca um usuário pelo e-mail (simula um SELECT com WHERE email = ?).
        // Retorna um UsuarioDto se encontrado, ou null se não existir.
        public UsuarioDto? BuscarUsuarioPorEmail(string email)
        {
            UsuarioDto usuarioRetorno = null;
            foreach (var usuario in usuarios)
            {
                if (usuario.Email == email)
                {
                    // Converte a entidade para DTO antes de retornar
                    usuarioRetorno = new UsuarioDto
                    {
                        Id = usuario.Id,
                        Nome = usuario.Nome,
                        Email = usuario.Email,
                        Ativo = usuario.Ativo,
                        DataCadastro = usuario.DataCadastro,
                        Funcao = usuario.Funcao,
                        Senha = usuario.Senha
                    };

                    break; // Encontrou, pode sair do loop
                }
            }

            return usuarioRetorno;
        }

        // Busca um usuário usando nome E e-mail ao mesmo tempo.
        // Os dois campos precisam combinar para retornar resultado (filtro combinado).
        public UsuarioDto? BuscarUsuarioPorNomeEEmail(string nome, string email)
        {
            UsuarioDto usuarioRetorno = null;
            foreach (var usuario in usuarios)
            {
                if (usuario.Email == email && usuario.Nome == nome)
                {
                    // Converte a entidade para DTO antes de retornar
                    usuarioRetorno = new UsuarioDto
                    {
                        Id = usuario.Id,
                        Nome = usuario.Nome,
                        Email = usuario.Email,
                        Ativo = usuario.Ativo,
                        DataCadastro = usuario.DataCadastro,
                        Funcao = usuario.Funcao,
                        Senha = usuario.Senha
                    };

                    break; // Encontrou, pode sair do loop
                }
            }

            return usuarioRetorno;
        }

        // Retorna todos os usuários da lista (simula um SELECT * FROM Usuarios).
        // Converte cada entidade interna para um DTO antes de retornar,
        // seguindo o padrão de não expor entidades internas diretamente.
        public List<UsuarioDto> ListarUsuarios()
        {
            List<UsuarioDto> usuariosRetorno = new List<UsuarioDto>();
            UsuarioDto usuarioDto;
            foreach (var usuario in usuarios)
            {
                // Converte cada entidade para DTO
                usuarioDto = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Ativo = usuario.Ativo,
                    DataCadastro = usuario.DataCadastro,
                    Funcao = usuario.Funcao,
                    Senha = usuario.Senha
                };

                usuariosRetorno.Add(usuarioDto);
            }

            return usuariosRetorno;
        }
    }
}
