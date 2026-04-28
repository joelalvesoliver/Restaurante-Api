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
                if(string.Equals(canal, "app", StringComparison.OrdinalIgnoreCase))
                {
                    var traceId = context.Request.Headers["traceId"].ToString();
                    Console.WriteLine($"[Trackin] Requisicao invalida, canal {canal} não suportado, traceId {traceId}");

                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            await _next(context);
        }
    }
}
