using System.Diagnostics;

namespace Restaurante.Api.Middlewares
{
    public class BloqueioHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public BloqueioHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/Cardapio"))
            {
                var canal = context.Request.Headers["X-Canal"].ToString();
                if(!string.Equals(canal, "app", StringComparison.OrdinalIgnoreCase))
                {
                    var traceId = context.Request.Headers["traceId"].ToString();
                    Console.WriteLine($"[Trackin] Requisicao invalida, canal {canal} não suportado, traceId {traceId}");
                   

                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            Console.WriteLine("Log 3 Entrada"); //Deve ser o terceiro a ser exibido na entrada
            await _next(context);
            Console.WriteLine("Log 3 sáida"); //Deve ser o primeiro a ser exibido na saída pois o middleware funcionara como uma pilha, ou seja, o código após o await _next(context) só será executado depois que os middlewares seguintes forem processados e a resposta estiver sendo enviada de volta para o cliente.
        }
    }
}
