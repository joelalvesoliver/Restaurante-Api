using Microsoft.AspNetCore.Mvc;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestesController : ControllerBase
    {
        // Endpoint simples para provocar ArgumentException e testar o filtro global
        // GET /api/testes/argument-exception
        [HttpGet("argument-exception")]
        public IActionResult GerarArgumentException()
        {
            throw new ArgumentException("Teste do filtro: ArgumentException disparada.");
        }
    }
}