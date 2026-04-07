using Microsoft.AspNetCore.Mvc;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : Controller
    {

        //api/Pedidos
        [HttpGet("")]
        public IActionResult Index()
        {
            return Content("Listar todos os pedidos");
        }

        /*  Exercício 1
        *   api/pedidos/mesa/{numeroMesa:int}
        */
        [HttpGet("mesa/{numeroMesa:int}")]
        public IActionResult BuscarMesaPeloNumero(int numeroMesa)
        {
            return Content($"Mesa número {numeroMesa}");
        }

        /*  Exercício 2
        *   api/pedidos/id/{id:guid}
        */
        [HttpGet("id/{Id:guid}")]
        public IActionResult ValidaGuid(Guid Id)
        {
            return Content($"Id {Id} Válido!");
        }

        /*  Exercício 3
        *   api/pedidos/IdPedido/{Id:int}/action/{acao}
        *   Obs.: (.net 10)
        *   Através da url: http://localhost:5207/api/Pedidos/IdPedido/1234/action/Cobranca
        *   o código executou normalmente gerando a saída: 
        *   "Ação de Cobranca executada para o pedido nº 1234"
        *   =========================================================
        *   Alterado para:
        *   api/pedidos/IdPedido/{Id:int}/operacao/{acao}
        *   Execução normal: http://localhost:5207/api/Pedidos/IdPedido/123456/operacao/Cancelamento
        *   saída: "Ação de Cancelamento executada para o pedido nº 123456"
        */
        [HttpGet("IdPedido/{Id:int}/action/{acao}")]
        //[HttpGet("IdPedido/{Id:int}/operacao/{acao}")]
        public IActionResult ExecutarAcao(int Id, string acao)
        {
            return Content($"Ação de {acao} executada para o pedido nº {Id}");
        }

    }
}
