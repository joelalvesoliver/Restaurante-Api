namespace Restaurante.Api.Middlewares
{
    public class BloqueioXHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public BloqueioXHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                var canal = context.Request.Headers["X-Turma"].ToString();
                if (string.IsNullOrEmpty(canal))
                {
                    var traceId = context.Request.Headers["traceId"].ToString();
                    Console.WriteLine($"[Trackin] Requisicao invalida, canal não suportado, traceId {traceId}");

                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }


            }

            await _next(context);
        }
    }
}

