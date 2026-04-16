using SimuladorBancoDados.Entidade;
using SimuladorBancoDados.Interfaces;

namespace SimuladorBancoDados.Service
{
    public class BancoDadosService : IBancoDados
    {
        private readonly List<Usuario> usuarios;
        private int proximoId = 1;
        public BancoDadosService()
        {
            usuarios = new List<Usuario>();
        }
        public void AdicionarNovoUsuario(UsuarioDto usuarioDto)
        {
            Usuario novoUsuario = new Usuario(usuarioDto, proximoId);
            usuarios.Add(novoUsuario);
            proximoId++;
        }

        public bool AutenticarUsuario(string email, string senha)
        {
            bool autenticado = false;
            foreach (var usuario in usuarios)
            {
                if (usuario.Email == email && usuario.Senha == senha)
                {
                    autenticado = true;
                    break;
                }
            }

            return autenticado;
        }

        public UsuarioDto? BuscarUsuarioPeloNome(string nome)
        {
            UsuarioDto usuarioRetorno = null;
            foreach (var usuario in usuarios)
            {
                if(usuario.Nome == nome)
                {
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

                    break;
                }
            }

            return usuarioRetorno;
        }

        public UsuarioDto? BuscarUsuarioPorEmail(string email)
        {
            UsuarioDto usuarioRetorno = null;
            foreach (var usuario in usuarios)
            {
                if (usuario.Email == email)
                {
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

                    break;
                }
            }

            return usuarioRetorno;
        }

        public List<UsuarioDto> ListarUsuarios()
        {
            List<UsuarioDto> usuariosRetorno = new List<UsuarioDto>();
            UsuarioDto usuarioDto;
            foreach (var usuario in usuarios)
            {
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
