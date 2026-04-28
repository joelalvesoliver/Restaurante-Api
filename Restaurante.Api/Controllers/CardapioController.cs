using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.DTOs;
using SimuladorBancoDados.Entidade;
using SimuladorBancoDados.Interfaces;
using Restaurante.Api.Services;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
    public class CardapioController : ControllerBase
    {
        private readonly IPratoRepository _pratoRep;
        private readonly ILogger<CardapioController> _logger;
        private readonly ArquivoService _arquivoService;
        //public CardapioController(ILogger<CardapioController> logger) 
        public CardapioController(IPratoRepository pratoRep, ILogger<CardapioController> logger, ArquivoService arquivoService) 
        {           
            _pratoRep = pratoRep;
            _logger = logger;
            _arquivoService = arquivoService;
        }

        /*
        * Projeto Final - Exercício 1 - Listar todos os pratos do cardápio
        * /api/cardapio
        */       
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ServiceFilter(typeof(LogAuditoria))]
        public IActionResult Index()
        {
            List<Prato> listaPratos = _pratoRep.BuscaTodosPratos(); 
            if(listaPratos.Count == 0)
            {
                _logger.LogInformation("[Lista Pratos] A busca não retornou nenhum registro!");
                return NotFound(new {Mensagem = "Nenhum prato localizado!"});
            }

            _logger.LogInformation("[Lista Pratos] Lista de todos os pratos (OK)");
            return Ok(listaPratos);
        }

        /*
        * Projeto Final - Exercício 2 - Obter prato específico
        * /api/cardapio/prato/{id}
        */
        [HttpGet("prato/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Prato> ObterPratoPeloId(int id)
        {
            if(id <= 0)
            {
                _logger.LogInformation("[Busca Prato] Identificador \"{0}\" inválido ", id);
                return BadRequest(new {mensagem = "Id inválido"});
            }

            var prato = _pratoRep.BuscaPratoPeloId(id);
            
            if(prato == null)
            {
                _logger.LogInformation("[Busca Prato] Prato Id={0} não encontrado!", id);
                return NotFound(new {mensagem = "Prato não encontrado! Verifique o identificador!"});
            }

            return Ok(prato);
        }

        /*
        * Projeto Final - Exercício 3: Cadastrar novo prato
        * api/cardapio/cadastrar-prato
        */
        [HttpPost("/cadastrar-prato")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PratoRespostaDTO> CadastrarPrato([FromBody] Prato prato)
        {
            _logger.LogInformation("Requisicao para cadastro de um novo prato");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Prato não pôde ser adicionado. Dadoso incompletos");
                return BadRequest(ModelState);
            }

            if (!_arquivoService.ArquivoExiste(prato.IdFoto))
            {
                _logger.LogError("Prato não pôde ser adicionado. Id da Foto inválida!");
                return BadRequest(new {Mensagem = "Id de arquivo inválido ou arquivo inexistente!"});
            }

            var pratoCriado = _pratoRep.AdicionaPrato(prato);
            String uri = $"/api/cardapio/prato/{pratoCriado.Id}";

            _logger.LogInformation("Prato {0} Criado com sucesso!", pratoCriado.Id);
  
            return Created(uri, new {
                Id = pratoCriado.Id,
                Nome = pratoCriado.Nome,
                Preco = pratoCriado.Preco,
                Descricao = pratoCriado.Descricao,
                Categoria = pratoCriado.Categoria,
                IdFoto = pratoCriado.IdFoto,
                UrlDownload = pratoCriado.UrlDownload
            });
        }

        /*
        * Projeto Final - Exercício 4: Upload de foto
        * api/cardapio/upload-foto
        */
        [HttpPost("/upload-foto")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ArqResponseSimplificadoDTO>> Upload([FromForm(Name = "arquivo")] IFormFile arquivo)
        {
            _logger.LogInformation("Requisicao de upload de arquivo: {NomeOriginal}", arquivo?.FileName);

            if(arquivo == null)
            {
                _logger.LogError("Erro: Nenhum arquivo fornecido!");
                return BadRequest(new {Mensagem = "Arquivo Não fornecido" });
            }

            var (sucesso, resultado) = await _arquivoService.UploadArquivoAsync(arquivo);

            if (!sucesso)
            {
                _logger.LogError("Erro realizando upload do arquivo!");
                return BadRequest(resultado);
            }

            ArqResponseSimplificadoDTO resp = new ArqResponseSimplificadoDTO();
            resp.NomeArmazenado = resultado.NomeArmazenado;
            resp.UrlDownload = resultado.UrlDownload;
            resp.Mensagem = resultado.Mensagem;

            _logger.LogInformation("Upload do arquivo {0} realizado com sucesso!", arquivo?.FileName);
  
            return Ok(resp);
        }

        /*
        * Projeto Final - Exercício 5: Download de foto do prato
        * api/cardapio/foto/{nomeArquivo}
        */

        [HttpGet("/foto/{nomeArquivo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DownloadFotoPrato(string nomeArquivo)
        {
            _logger.LogInformation("Requisicao para download de arquivo: {NomeArquivo}", nomeArquivo);

            var stream = _arquivoService.ObterArquivoParaDownload(nomeArquivo);
            if(stream == null)
            {
                _logger.LogError("Arquivo de nome: {NomeArquivo} não encontrado!", nomeArquivo);
                return NotFound(new { Mensagem = "Arquivo não encontrado" });
            }

            var contetType = UtilitariosService.GetMimeType(nomeArquivo);
            return File(stream, contetType, nomeArquivo);
        }

    }
}


/*
 OBS: Palavras reservadas, evitar usa - las na construção das rotas
 action
 controller
 area
 page
 handler

 
200 OK: leitura ou operacao concluida com retorno;
201 Created: recurso criado com sucesso;
204 NoContent: sucesso sem corpo de resposta;

400 BadRequest: erro de validacao/entrada;
401 Unauthorized: nao autenticado;
403 Forbidden: autenticado, mas sem permissao;
404 NotFound: recurso nao encontrado;

500 InternalServerError: erro inesperado no servidor.
503 Serviço indiponível
504 timeout, demorou para responder
 
 */