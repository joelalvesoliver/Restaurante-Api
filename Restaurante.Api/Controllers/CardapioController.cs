using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.Filtros;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Service;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
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
        //3f2504e0-4f89-11d3-9a0c-0305e82c3301
        [HttpGet("prato-guid/{id:guid}")]
        public IActionResult ObterPratoPorGuid(Guid id)
        {
            return Content($"Busca do prato com o guid {id}");
        }
        // api/cardapio/categoria/saladas/pagina/1
        [HttpGet("categoria/{nomeCategoria}/pagina/{pagina:int}")]
        [TypeFilter(typeof(VericarCacheFilter))]
        public IActionResult ListarPorCategoria(string nomeCategoria, int pagina)
        {
            return Content($"Categoria {nomeCategoria}, pagina {pagina}");
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