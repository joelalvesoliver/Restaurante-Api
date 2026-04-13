using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.Filtros;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        // ==========================================================
        // 1.1 Exercício 1 - Segmentos + constraint int
        // GET /api/pedidos/mesa/{numeroMesa:int}
        // Requisitos:
        // - Retornar texto com numero da mesa
        // - Validar comportamento para valor numérico e não numérico
        // ==========================================================
        [HttpGet("mesa/{numeroMesa:int}")]
        public IActionResult ObterPorMesa(int numeroMesa)
        {
            return Ok($"Pedidos da mesa: {numeroMesa}");
        }

        // ==========================================================
        // 1.2 Exercício 2 - Segmento com guid
        // GET /api/pedidos/{id:guid}
        // Requisitos:
        // - Se GUID válido, retornar texto de confirmação
        // - Se não for GUID, rota não deve casar
        // ==========================================================
        [HttpGet("{id:guid}")]
        public IActionResult ObterPorId(Guid id)
        {
            return Ok($"GUID válido! Pedido localizado com sucesso. ID: {id}");
        }

        // ==========================================================
        // 1.3 Exercício 3 - Palavras reservadas
        // 1) Criar rota usando parâmetro chamado "action" (significado negócio)
        // 2) Observar confusões
        // 3) Refatorar para outro nome e justificar
        // ==========================================================

        // (VERSÃO "PROPOSITALMENTE RUIM" para análise)
        // GET /api/pedidos/acao/{action}
        // Observação: usar "action" pode confundir com token/conceito de action do MVC.
        [HttpGet("acao/{action}")]
        public IActionResult AcaoComNomeReservado(string action)
        {
            return Ok($"Ação de negócio recebida (param='action'): {action}");
        }

        // (VERSÃO REFATORADA - recomendada)
        // GET /api/pedidos/acao-refatorada/{acaoPedido}
        // Justificativa: "acaoPedido" deixa claro ser dado de domínio e evita ambiguidade com "action".
        [HttpGet("acao-refatorada/{acaoPedido}")]
        public IActionResult AcaoRefatorada(string acaoPedido)
        {
            return Ok($"Ação de negócio recebida (param='acaoPedido'): {acaoPedido}");
        }
 
        /* Aula 03 - Exercício 1
        * GET /api/pedidos/id:int}
        * Rota criada para que exista um pedido com id inteiro pra ser validado pelo filtro
        */
        [HttpGet("{Id:int}")]
        [ServiceFilter(typeof(ValidaIdPositivoFilter))]
        public IActionResult VerificarPedido(int Id)
        {
            /* Aula 03 - Exercício 3
            * A chamada abaixo simula uma Argument Exception de forma a testar o filtro correspondente
            */
            //throw new ArgumentException();
            return Ok($"Pedido Com identificador: {Id} OK!");
        } 
    }
}