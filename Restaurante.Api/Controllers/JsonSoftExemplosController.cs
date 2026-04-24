using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.DTOs;
using SimuladorBancoDados;
// Biblioteca externa para trabalhar com JSON (mais recursos que a nativa do .NET).
// Newtonsoft.Json é uma das bibliotecas mais usadas no ecossistema .NET.
using Newtonsoft.Json;
// Biblioteca NATIVA do .NET para trabalhar com JSON (introduzida no .NET Core 3.0+).
using System.Text.Json;

namespace Restaurante.Api.Controllers
{
    // [ApiController]: define esta classe como um controller de API.
    [ApiController]
    // URL base: "api/exemplos-jsonsoft" (rota definida manualmente, sem usar [controller]).
    [Route("api/exemplos-jsonsoft")]

    // DTO auxiliar para receber um payload JSON como texto puro.
    // Usamos esta classe para que o cliente possa enviar um JSON como string,
    // permitindo demonstrar como "deserializar" (converter JSON → objeto C#).
    public class JsonPayloadDTO
    {
        // Campo que conterá o texto JSON enviado pelo cliente.
        // "string.Empty" como valor padrão evita que o campo seja null.
        public string Json { get; set; } = string.Empty;
    }

    // Controller com exemplos didáticos de serialização e deserialização JSON.
    //
    // O que é JSON?
    // JSON (JavaScript Object Notation) é o formato de troca de dados mais usado em APIs.
    // É legível por humanos e suportado por praticamente todas as linguagens.
    // Exemplo: { "nome": "Maria", "email": "maria@ada.com" }
    //
    // SERIALIZAÇÃO: converter um objeto C# → texto JSON.
    // DESERIALIZAÇÃO: converter texto JSON → objeto C#.
    public class JsonSoftExemplosController : ControllerBase
    {
        // GET api/exemplos-jsonsoft/serializacao
        // Demonstra como converter um objeto C# em texto JSON.
        //
        // Por que serializar?
        // Quando a API precisa enviar dados para o cliente, os objetos C# são
        // automaticamente convertidos (serializados) para JSON pelo framework.
        // Este endpoint mostra esse processo de forma explícita e didática.
        [HttpGet("serializacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult ExemploSerializacao()
        {
            // Cria um objeto FuncionarioRespostaDto com dados de exemplo.
            var funcionario = new FuncionarioRespostaDto
            {
                Id = 1,
                Nome = "Maria",
                Email = "email.maria@ada.com",
                Funcao = Funcao.Gerente,
                Ativo = true,
                DataCadastro = DateTime.Now,
            };

            // JsonConvert.SerializeObject: converte o objeto para uma string JSON formatada.
            // "Formatting.Indented" formata o JSON com indentação para facilitar a leitura.
            // Sem isso, o JSON seria gerado em uma única linha (minificado).
            var jsonSerializado = JsonConvert.SerializeObject(funcionario, Formatting.Indented);

            // Retorna tanto o objeto original quanto a versão serializada em texto,
            // para que o aluno possa comparar os dois formatos.
            return Ok(new
            {
                conceito = "serializacao",
                objetoOriginal = funcionario,
                jsonSerializado = jsonSerializado
            });
        }

        // POST api/exemplos-jsonsoft/desserializacao
        // Demonstra como converter texto JSON de volta para um objeto C#.
        //
        // Por que deserializar?
        // Quando o cliente envia dados para a API, eles chegam como texto JSON.
        // O framework deserializa automaticamente, mas este endpoint mostra o processo
        // manualmente, usando duas bibliotecas diferentes como comparação.
        [HttpPost("desserializacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult ExemploDesserializacao([FromBody] JsonPayloadDTO payload)
        {
            // Valida se o campo "json" foi enviado e não está vazio.
            if (string.IsNullOrWhiteSpace(payload.Json))
            {
                return BadRequest(new { mensagem = "Informe o campo 'json' com um JSON valido." });
            }

            try
            {
                // OPÇÃO 1: Deserialização usando a biblioteca NATIVA do .NET (System.Text.Json).
                // Disponível desde o .NET Core 3.0. É mais performática, mas menos flexível.
                var login1 = System.Text.Json.JsonSerializer.Deserialize<LoginDTO>(payload.Json);
                
                // OPÇÃO 2: Deserialização usando a biblioteca EXTERNA Newtonsoft.Json.
                // Mais antiga e flexível, com mais opções de configuração.
                // Ambas fazem o mesmo trabalho; a escolha depende do contexto do projeto.
                var login = JsonConvert.DeserializeObject<LoginDTO>(payload.Json);
                
                // OPÇÃO 3: Parse manual do JSON (sem converter para uma classe específica).
                // Útil quando precisamos ler apenas um campo específico sem criar um DTO.
                // JsonDocument permite navegar pelo JSON como uma árvore de elementos.
                using (JsonDocument doc = JsonDocument.Parse(payload.Json))
                {
                    // RootElement é a raiz do documento JSON (o objeto principal).
                    JsonElement root = doc.RootElement;
                    // GetProperty busca um campo pelo nome; GetString() extrai o valor como string.
                    string senha = root.GetProperty("Senha").GetString();
                    string email = root.GetProperty("Email").GetString();
                }

                // Verifica se a deserialização produziu um objeto válido.
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
                // Se o JSON enviado for inválido (sintaxe errada, campos ausentes, etc.),
                // uma exceção é lançada e retornamos um 400 com a mensagem de erro.
                return BadRequest(new
                {
                    mensagem = "JSON invalido para o formato esperado de LoginDTO.",
                    detalhe = ex.Message
                });
            }
        }
    }
}
