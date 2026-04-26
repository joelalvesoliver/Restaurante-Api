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
            if (numeroMesa <= 0)
            {
                return BadRequest("O número da mesa deve ser um valor positivo.");
            }

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
        */
        [HttpGet("IdPedido/{Id:int}/action/{acao}")]
        public IActionResult ExecutarAcao(int Id, string acao)
        {
            return Content($"Ação de {acao} executada para o pedido nº {Id}");
        }

    }
}
// Exercício 3: Palavras Reservadas
// No ecossistema ASP.NET MVC, o termo action é considerado uma palavra reservada por convenção, pois é amplamente utilizado para representar o método do controller, conhecido como action method. O uso de action como nome de parâmetro em rotas pode reduzir a clareza do código, dificultando a leitura e a compreensão do seu real significado de negócio. Além disso, dependendo da configuração de rotas da aplicação, especialmente quando há rotas convencionais, essa escolha pode gerar ambiguidades ou conflitos com o mecanismo de roteamento do framework. Diante disso, recomenda-se a refatoração do parâmetro para nomes como operacao ou acao, que possuem significado mais alinhado ao domínio da aplicação, tornam a intenção do código mais explícita e evitam possíveis confusões técnicas e semânticas.