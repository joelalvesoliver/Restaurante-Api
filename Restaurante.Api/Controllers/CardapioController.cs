using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.Filtros;
using Restaurante.Api.Services;
using Restaurante.Api.DTOs;
using SimuladorBancoDados.DTOs;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Service;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
    public class CardapioController : ControllerBase
    {
        public CardapioController() 
        {           
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
        [ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
        public IActionResult ObterPratoPeloId(int id)
        {
            return Content($"Detalhes do prato de id {id}");
        }

        private readonly IPratoRepository _service;
        private readonly ILogger<CardapioController> _logger;
        private readonly ArquivoService _arquivoService;

        public CardapioController(IPratoRepository service, ILogger<ArquivosController> logger, ArquivoService arquivoService) 
        {
            _arquivoService = arquivoService;
            _logger = logger;
            _service = service;
        }
        
        [HttpGet]
       // [ServiceFilter(typeof(LogAuditoria))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Index()
        {
            _logger.LogInformation("Iniciando busca de todos os pratos do cardápio.");

            var pratosDomain = _service.BuscaTodosPratos();

            if (pratosDomain == null || !pratosDomain.Any())
            {
                _logger.LogWarning("Nenhum prato encontrado no cardápio.");
                return NoContent();
            }

            var listaDto = pratosDomain.Select(prato => new PratoResponseDTO
            {
                Id = prato.Id,
                Nome = prato.Nome,
                Preco = prato.Preco,
                Descricao = prato.Descricao,
                Categoria = prato.Categoria,
                DataCadastro = prato.DataCadastro,
                FotoId = prato.FotoId,
                UrlDownloadFoto = prato.FotoId.HasValue
                    ? $"{Request.Scheme}://{Request.Host}/api/fotos/download/{prato.FotoId}"
                    : null
            });

            return Ok(listaDto);
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