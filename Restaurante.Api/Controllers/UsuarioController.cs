using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.DTOs;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        List<FuncionarioRespostaDto> funcionarios;
        public UsuarioController() {
            funcionarios = new List<FuncionarioRespostaDto>()
            {
                new FuncionarioRespostaDto { Email = "joel.alves@gmail.com", Nome = "Joel alves"},
                new FuncionarioRespostaDto {Email = "joel.alves1@gmail.com", Nome = "Joel alves"},
                new FuncionarioRespostaDto {Email = "joaquim@gmail.com", Nome = "Joaquim"},
                new FuncionarioRespostaDto {Email = "ricardo@gmail.com", Nome = "Ricardo"}
            };
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

            funcionarios.Add(resposta);
            //return Ok(resposta);
            return CreatedAtRoute("BuscarUsuario", new { nome = resposta.Nome }, resposta);
        }


        [HttpGet("{nome}", Name = "BuscarUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FuncionarioRespostaDto> BuscarUsuario(string nome)
        {
            foreach(var funcionario in funcionarios)
            {
                if(funcionario.Nome == nome)
                {
                    return Ok(funcionario);
                }
            }

            return NotFound(
                new
                {
                    mensagem = "Usuario não encontrado!",
                }); 
        }

            [HttpGet] // atualizo uma ou mais informações do usuario
        //[HttpPut] // atualizo todos os dados do usuario
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    }
}


