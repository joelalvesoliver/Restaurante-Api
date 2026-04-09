using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    public class EnvolveRespostaFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("Filtro de resultado executado");

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // 1. Pega o status code atual
            int statusCode = context.HttpContext.Response.StatusCode;

            // Pequeno ajuste na lógica: Status 200 também é sucesso!
            bool sucesso = statusCode >= 200 && statusCode < 300;

            // 2. Extrai os dados reais do IActionResult
            object dadosPuros = null;

            if (context.Result is ObjectResult objectResult)
            {
                dadosPuros = objectResult.Value;
            }
            else if (context.Result is JsonResult jsonResult)
            {
                dadosPuros = jsonResult.Value;
            }
            // Se for um EmptyResult ou algo sem valor, dadosPuros continuará null

            // 3. Monta o envelope
            var envelope = new
            {
                sucesso = sucesso,
                dados = dadosPuros, // Agora passamos o objeto real, não o IActionResult
                mensagem = sucesso ? "Operação realizada com sucesso" : "Operação realizada com falha"
            };

            // 4. Sobrescreve o resultado com o novo ObjectResult
            context.Result = new ObjectResult(envelope)
            {
                StatusCode = statusCode
            };
        }
    }
}
