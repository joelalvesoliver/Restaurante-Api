namespace Restaurante.Api.Middlewares
{
    public class BloqueioHorarioMiddleware
    {
        private readonly RequestDelegate _next;

        public BloqueioHorarioMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            var method = context.Request.Method;

            if (path == "/api/pedidos" && method == HttpMethods.Post)
            {
                var horaAtual = DateTime.Now.Hour;

                if (horaAtual < 11 || horaAtual >= 23)
                {
                    context.Response.StatusCode = StatusCodes.Status423Locked; // ou 403
                    await context.Response.WriteAsync(
                        "Pedidos só podem ser realizados entre 11h e 23h."
                    );
                    return;
                }
            }

            await _next(context);
        }
    }
}
