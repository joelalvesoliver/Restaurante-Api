using Microsoft.AspNetCore.Mvc;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidosController : Controller
    {
        //api/pedidos/mesa/{numeroMesa}
        [HttpGet("mesa/{numeroMesa:int}")]
        public IActionResult FazerPedido(int numeroMesa)
        {
            if (numeroMesa > 0)
            {
                return Content($"Número da mesa: {numeroMesa}");
            }

            return BadRequest("O número da mesa deve ser positivo.");

            /* constraint :int basta para o ASP.NET Core aceitar apenas valores numéricos na rota.
            Para implantar validação de valor não numérico, a propriedade numeroMesa deveria ser uma string
            e desta forma tentar converter com tryparse para validar */
        }


        //api/pedidos/mesa/{numMesa:guid}
        [HttpGet("mesa/{numMesa:guid}")]
        public IActionResult TirarPedido(Guid numMesa)
        {
            //verifica se o guid é vazio = 0
            if (numMesa == Guid.Empty)
            {
                return BadRequest("O Guid informado não é válido.");
            }

            return Content($"Número da mesa: {numMesa}");

            /* constraint :guid basta para o ASP.NET Core aceitar apenas valores guid na rota,
            caso contrário a rota não é encontrada - 3f2504e0-4f89-11d3-9a0c-0305e82c3301*/
        }

        //api/pedidos/servico/{action}
        [HttpGet("servico/{action}")]
        public IActionResult ExecutarServico(string action)
        {
            return Content($"Serviço finalizado: {action}");
        }

        /* com parâmetro "action" nenhum valor de entrada é aceito = Error: response status is 404 */


        //api/pedidos/negocio/{operacao}
        [HttpGet("negocio/{operacao}")]
        public IActionResult ExecutarNegocio(string operacao)
        {
            return Content($"Negócio fechado: {operacao}");
        }

        /* funciona */
    }
}
