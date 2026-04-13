using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api
{
    public class AdicionaHoraResposta : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            // não é necessário
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            context.HttpContext.Response.Headers.Add("X-Processado-Em", timestamp);
        }
    }
}
