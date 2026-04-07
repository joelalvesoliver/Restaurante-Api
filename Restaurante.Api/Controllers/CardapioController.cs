using Microsoft.AspNetCore.Mvc;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardapioController : Controller
    {

        //api/cardapio
        [HttpGet("")]
        public IActionResult Index()
        {

            return Content("Lista de pratos do cardapio");
        }

        //api/cardapio/prato/{id}
        [HttpGet("prato/{id}")]
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
*/