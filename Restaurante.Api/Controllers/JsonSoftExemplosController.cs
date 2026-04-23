using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.DTOs;
using SimuladorBancoDados;
using Newtonsoft.Json;
using System.Text.Json;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/exemplos-jsonsoft")]

    public class JsonPayloadDTO
    {
        public string Json { get; set; } = string.Empty;
    }
    public class JsonSoftExemplosController : ControllerBase
    {
        [HttpGet("serializacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult ExemploSerializacao()
        {
            var funcionario = new FuncionarioRespostaDto
            {
                Id = 1,
                Nome = "Maria",
                Email = "email.maria@ada.com",
                Funcao = Funcao.Gerente,
                Ativo = true,
                DataCadastro = DateTime.Now,
            };

            var jsonSerializado = JsonConvert.SerializeObject(funcionario, Formatting.Indented);

            return Ok(new
            {
                conceito = "serializacao",
                objetoOriginal = funcionario,
                jsonSerializado = jsonSerializado
            });
        }

        [HttpPost("desserializacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult ExemploDesserializacao([FromBody] JsonPayloadDTO payload)
        {
            if (string.IsNullOrWhiteSpace(payload.Json))
            {
                return BadRequest(new { mensagem = "Informe o campo 'json' com um JSON valido." });
            }

            try
            {
                // usando lib nativa do .Net
                var login1 = System.Text.Json.JsonSerializer.Deserialize<LoginDTO>(payload.Json);
                
                // usando lib externa
                var login = JsonConvert.DeserializeObject<LoginDTO>(payload.Json);
                
                // Forma de pegar os valores dando um parse no json e recuperando os dados em variaveis
                using (JsonDocument doc = JsonDocument.Parse(payload.Json))
                {
                    JsonElement root = doc.RootElement;
                    string senha = root.GetProperty("Senha").GetString();
                    string email = root.GetProperty("Email").GetString();
                }

                if (login.Email == null || login.Senha == null)
                {
                    return BadRequest(new { mensagem = "Nao foi possivel converter o JSON para LoginDTO." });
                }

                return Ok(new
                {
                    conceito = "desserializacao",
                    jsonRecebido = payload.Json,
                    objetoConvertido = login
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = "JSON invalido para o formato esperado de LoginDTO.",
                    detalhe = ex.Message
                });
            }
        }
    }
}
