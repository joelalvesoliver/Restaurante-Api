using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    public class VericarCacheFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // exectar depois da chamada da rota
            Console.WriteLine("Atualiza o cache");
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // irá executar antes da chamada da rota
            Console.WriteLine("Verificar no cache");
            context.HttpContext.Request.Headers["cache"] = "";
        }
    }
}
