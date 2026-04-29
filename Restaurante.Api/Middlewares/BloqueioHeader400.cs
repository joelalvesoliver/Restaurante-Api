using System.Diagnostics;

namespace Restaurante.Api.Middlewares
{
    public class BloqueioHeader400Middleware
    {

        private readonly RequestDelegate _next;

        public BloqueioHeader400Middleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                var canal = context.Request.Headers["X-Turma"].ToString();
                if (!string.Equals(canal, "app", StringComparison.OrdinalIgnoreCase))
                {
                    var traceId = context.Request.Headers["traceId"].ToString();
                    Console.WriteLine($"[Trackin] Requisicao invalida, canal {canal} não suportado, traceId {traceId}");

                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
            }





            Console.WriteLine("Log 2 Entrada"); //Deve ser o segundo a ser exibido na entrada
            await _next(context);
            Console.WriteLine("Log 2 sáida"); //Deve ser o segundo a ser exibido na saída pois o middleware funcionara como uma pilha, ou seja, o código após o await _next(context) só será executado depois que os middlewares seguintes forem processados e a resposta estiver sendo enviada de volta para o cliente.
        }
    }
}




