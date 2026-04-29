using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Restaurante.Api.DTOs;
using SimuladorBancoDados;
using SimuladorBancoDados.Interfaces;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IBancoDados _service;
        private readonly IConfiguration _configuration;
        List<FuncionarioRespostaDto> funcionarios;


        // Exercicio 1 Aula 3
        [HttpGet("teste/{id:int}")]
        public IActionResult TesteId(int id) { return Ok(new { mensagem = "ID válido", id }); }


        // Exercicio 3 Aula 3
        [HttpGet("teste-exception")]
        public IActionResult Teste()
        {
            throw new ArgumentException("ID inválido para teste");
        }
        public UsuarioController(IBancoDados service, IConfiguration configuration) {
            funcionarios = new List<FuncionarioRespostaDto>()
            {
                new FuncionarioRespostaDto { Email = "joel.alves@gmail.com", Nome = "Joel alves"},
                new FuncionarioRespostaDto {Email = "joel.alves1@gmail.com", Nome = "Joel alves"},
                new FuncionarioRespostaDto {Email = "joaquim@gmail.com", Nome = "Joaquim"},
                new FuncionarioRespostaDto {Email = "ricardo@gmail.com", Nome = "Ricardo"}
            };
            _configuration = configuration;
            _service = service;

        }

        // FromBody
        // FromQuery
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<FuncionarioRespostaDto> CriarUsuario(
            [FromBody] FuncionarioDTO funcionario,
            [FromHeader(Name = "X-Perfil")] string perfil)
        {


            if (string.IsNullOrEmpty(perfil))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new
                    {
                        sucesso = false,
                        codigo = StatusCodes.Status403Forbidden,
                        mensagem = "Header X-Perfil não informado."
                    });
            }

            if (!string.Equals(perfil, "Gerente", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new
                    {
                        sucesso = false,
                        codigo = StatusCodes.Status403Forbidden,
                        mensagem = "Apenas gerente tem acesso para cadastrar novos funcionarios!"
                    });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resposta = new FuncionarioRespostaDto
            {
                Id = 1,
                Nome = funcionario.Nome,
                Email = funcionario.Email,
                Funcao = funcionario.Funcao,
                Ativo = true,
                DataCadastro = DateTime.Now,
            };

            _service.AdicionarNovoUsuario(new UsuarioDto
            {
                Nome = funcionario.Nome,
                Email = funcionario.Email,
                Funcao = funcionario.Funcao,
                DataCadastro = DateTime.Now,
                Ativo = true,
                Senha = funcionario.Senha
            });
            
            return CreatedAtRoute("BuscarUsuario", new { nome = resposta.Nome }, resposta);
        }


        [HttpGet("{nome}", Name = "BuscarUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FuncionarioRespostaDto> BuscarUsuario(string nome)
        {
            //foreach(var funcionario in funcionarios)
            //{
            //    if(funcionario.Nome == nome)
            //    {
            //        return Ok(funcionario);
            //    }
            //}

            //return NotFound(
            //    new
            //    {
            //        mensagem = "Usuario não encontrado!",
            //    }); 

            var usuario = _service.BuscarUsuarioPeloNome(nome);
            if(usuario == null)
            {
                return NotFound(
                new
                {
                    mensagem = "Usuario não encontrado!",
                });
            }

            return Ok(usuario);
        }

        [HttpPatch] // atualizo uma ou mais informações do usuario
        //[HttpPut] // atualizo todos os dados do usuario
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Gerente")]
        public ActionResult<FuncionarioRespostaDto> AtualizarUsuario(
            [FromQuery] string? nome, [FromQuery] string? email)
        {
            List<FuncionarioRespostaDto> funcionariosResultado = new List<FuncionarioRespostaDto>();
            
            //aplicou filtro do nome e email
            if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(email))
            {
                foreach (var item in funcionarios)
                {
                    if (item.Nome == nome && item.Email == email)
                        funcionariosResultado.Add(new FuncionarioRespostaDto
                        {
                            Email = item.Email,
                            Nome = item.Nome,
                        });
                }

                return Ok(funcionariosResultado);
            }

            //apliquei filtro do nome
            if (!string.IsNullOrEmpty(nome))
            {
                foreach (var item in funcionarios)
                {
                    if (item.Nome == nome)
                        funcionariosResultado.Add(new FuncionarioRespostaDto
                        {
                            Email = item.Email,
                            Nome = item.Nome,
                        });
                }

                return Ok(funcionariosResultado);
            }

            // apliquei o filtro do email
            if (!string.IsNullOrEmpty(email))
            {
                foreach (var item in funcionarios)
                {
                    if (item.Email == email)
                        funcionariosResultado.Add(new FuncionarioRespostaDto
                        {
                            Email = item.Email,
                            Nome = item.Nome,
                        });
                }

                return Ok(funcionariosResultado);
            }

            // se cheguei aqui eu não apliquei nenhum filtro
            foreach (var item in funcionarios)
            {

                 funcionariosResultado.Add(new FuncionarioRespostaDto
                 {
                    Email = item.Email,
                    Nome = item.Nome,
                 });
            }
            return Ok(funcionariosResultado);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<FuncionarioRespostaDto> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = _service.BuscarUsuarioPorEmail(dto.Email);
            if(usuario == null)
            {
                return Unauthorized(new 
                { 
                    mensagem = "Email ou senha inválidos"
                });
            }

            var autenticado = _service.AutenticarUsuario(dto.Email, dto.Senha);
            if (!autenticado)
            {
                return Unauthorized(new
                {
                    mensagem = "Email ou senha inválidos"
                });

            }

            // gerar o token
            var token = GerarToken(usuario, out DateTime expiracao);

            return Ok( new
            {
                usuario = usuario,
                token = token,
                expiracao = expiracao
            });
        }



        private string GerarToken(UsuarioDto usuario, out DateTime expiracao)
        {
            var jwtSection = _configuration.GetSection("Jwt");

            var jwtKey = jwtSection["Key"];
            var jwtIssuer = jwtSection["Issuer"];
            var jwtAudience = jwtSection["Audience"];
            var expiraEmMinutos = int.TryParse(jwtSection["ExpiresInMinutes"], out var valor)
                ? valor
                : 120;

            var funcao = Enum.IsDefined(typeof(Funcao), usuario.Funcao) 
                ? ((Funcao)usuario.Funcao).ToString() : usuario.Funcao.ToString();
            
            
            var claims = new List<Claim>();

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, usuario.Email.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, usuario.Nome));
            claims.Add(new Claim(ClaimTypes.Role, funcao));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            expiracao = DateTime.Now.AddMinutes(expiraEmMinutos);

            var tokenJwt = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiracao,
                signingCredentials: credenciais

            );

            return new JwtSecurityTokenHandler().WriteToken(tokenJwt);
        }
    }
}


