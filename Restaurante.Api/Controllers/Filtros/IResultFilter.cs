using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filters
{
    public class AdicionarHeaderProcessadoEmFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Adiciona o header "X-Processado-Em" com o timestamp atual
            context.HttpContext.Response.Headers.Add("X-Processado-Em", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Não é necessário implementar nada aqui para este caso
        }
    }
}