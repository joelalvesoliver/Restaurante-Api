namespace Restaurante.Api.Middlewares
{
    public class CurtoCircuitoMiddleware
    {
        private readonly RequestDelegate _next;

        public CurtoCircuitoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if ((context.Request.Method.Equals("POST")) && (context.Request.Path.StartsWithSegments("/api/pedidos")))
            {
                TimeOnly inicio = new TimeOnly(11, 0);
                TimeOnly fim = new TimeOnly(23, 0);
                TimeOnly agora = TimeOnly.FromDateTime(DateTime.Now);

                // Verifica se está entre
                if (!agora.IsBetween(inicio, fim))
                {
                    var traceId = context.Request.Headers["traceId"].ToString();
                    Console.WriteLine($"[Trackin] Requisicao invalida, fora do horário, traceId {traceId}");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }

            }

            await _next(context);
        }
    }
}

    