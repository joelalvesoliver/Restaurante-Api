using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filters
{
    public class AdicionarHeaderProcessadoEmFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Adiciona ou sobrescreve o header com a data/hora do processamento
            context.HttpContext.Response.Headers["X-Processado-Em"] =
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Não é necessário implementar nada aqui para este exercício
        }
    }
}