using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.DTOs;
using Restaurante.Api.Filtros;
using Restaurante.Api.Services;
using SimuladorBancoDados.Entidade;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Service;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
    public class CardapioController : ControllerBase
    {
        private readonly ArquivoService _arquivoService;
        private readonly ILogger<ArquivosController> _logger;
        private readonly IPratoRepository _service;

        public CardapioController(ArquivoService arquivoService, ILogger<ArquivosController> logger, IPratoRepository service) 
        {
            _arquivoService = arquivoService;
            _logger = logger;
            _service = service;
        }

        [HttpGet("listarPratos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<List<PratoResponseDTO>> Listar()
        {
            _logger.LogInformation("Requisicao para listar pratos.");
            var pratos = _service.BuscaTodosPratos();

            if (pratos.Count == 0)
            {
                _logger.LogInformation("Lista vazia");
                return NoContent();
            }
            var retorno = new List<PratoResponseDTO>();
            foreach(var prato in pratos)
            {
                PratoResponseDTO dados = new PratoResponseDTO();
                dados.Id = prato.Id;
                dados.Nome = prato.Nome;
                dados.Preco = prato.Preco;
                dados.Descricao = prato.Descricao;
                dados.Categoria = prato.Categoria;
                dados.DataCadastro  = prato.DataCadastro;
                dados.IdFoto = prato.IdFoto;
                dados.Endpoint = _arquivoService.CaminhoArquivo(prato.IdFoto);
            }
            _logger.LogInformation("Sucesso, lista emitida com sucesso.");
            return Ok(pratos);
        }

        //api/cardapio
        [HttpGet("")]
        [ServiceFilter(typeof(LogAuditoria))]
        public IActionResult Index()
        {
            return Content("Lista de pratos do cardapio");
        }

        //api/cardapio/prato/{id}
        
        [HttpGet("prato/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PratoResponseDTO> ObterPratoPeloId(int id)
        {
            _logger.LogInformation("Requisicao para listar prato pelo Id.");
            if (id <= 0)
            {
                _logger.LogInformation("Id inválido");
                return NotFound();
            }
            var retorno = _service.BuscaPratoPeloId(id);
            if (retorno == null)
            {
                _logger.LogInformation("Prato não existe");
                return NotFound();
            }
            else
            { 
                PratoResponseDTO dados = new PratoResponseDTO();
                dados.Id = retorno.Id;
                dados.Nome = retorno.Nome;
                dados.Preco = retorno.Preco;
                dados.Descricao = retorno.Descricao;
                dados.Categoria = retorno.Categoria;
                dados.DataCadastro = retorno.DataCadastro;
                dados.IdFoto = retorno.IdFoto;
                dados.Endpoint = _arquivoService.CaminhoArquivo(retorno.IdFoto);
                _logger.LogInformation("Sucesso, prato localizado.");
                return Ok(dados);
            }
        }

        [HttpPost("cadastrarPrato/{prato}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PratoResponseDTO> CriarPrato(CreatePratoDTO prato)
        {
            _logger.LogInformation("Requisicao para cadastrar prato");
            if (prato == null)
            {
                _logger.LogInformation("Prato inválido");
                return BadRequest();
            }
            if(!_arquivoService.ArquivoExiste(prato.IdFoto))
            {
                _logger.LogInformation("Foto do prato não existe");
                return BadRequest();
            }
            var novoPrato = new Prato();
            novoPrato.Nome = prato.Nome;
            novoPrato.Preco = prato.Preco;
            novoPrato.Descricao = prato.Descricao;
            novoPrato.Categoria = prato.Categoria;
            novoPrato.IdFoto = prato.IdFoto;
            _service.AdicionaPrato(novoPrato);
            _logger.LogInformation("Sucesso, prato criado.");
            return Ok(novoPrato);
        }

        [HttpPost("upload-foto")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ArquivoResponseDTO>> Upload([FromForm(Name = "arquivo")] IFormFile arquivo)
        {
            _logger.LogInformation("Recebida requisicao de upload de arquivo: {NomeOriginal}", arquivo?.FileName);

            if (arquivo == null)
            {
                return BadRequest(new { Mensagem = "Nenhum arquivo foi fornecido" });
            }

            var (sucesso, resultado) = await _arquivoService.UploadArquivoAsync(arquivo);
 
            if (!sucesso)
            {
                return BadRequest(resultado);
            }
            return Ok(resultado);
        }

        [HttpGet("download/{nomeArquivo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult Download(string nomeArquivo)
        {
            _logger.LogInformation("Recebida requisicao de download de arquivo");
            var stream = _arquivoService.ObterArquivoParaDownload(nomeArquivo);
            if (stream == null)
            {
                return NotFound(new { Mensagem = "Arquivo não encontrado" });
            }

            var contetType = GetMimeType(nomeArquivo);
            return File(stream, contetType, nomeArquivo);
        }

        private static string GetMimeType(string nomeArquivo)
        {
            var extensao = Path.GetExtension(nomeArquivo).ToLowerInvariant();
            return extensao switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".txt" => "text/plain",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }

        //3f2504e0-4f89-11d3-9a0c-0305e82c3301
        //[HttpGet("prato-guid/{id:guid}")]
        //public IActionResult ObterPratoPorGuid(Guid id)
        //{
        //    return Content($"Busca do prato com o guid {id}");
        //}
        // api/cardapio/categoria/saladas/pagina/1
        //[HttpGet("categoria/{nomeCategoria}/pagina/{pagina:int}")]
        //[TypeFilter(typeof(VericarCacheFilter))]
        //public IActionResult ListarPorCategoria(string nomeCategoria, int pagina)
        //{
        //    return Content($"Categoria {nomeCategoria}, pagina {pagina}");
        //}
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