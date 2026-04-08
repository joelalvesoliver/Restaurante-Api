namespace Restaurante.Api.Middlewares
{
    public class HorarioPedidoMiddleware
    {
        private readonly RequestDelegate _next;

        // Define o intervalo permitido (11h às 23h)
        private readonly int _horaInicio = 11;
        private readonly int _horaFim = 23;

        public HorarioPedidoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Verifica se é uma requisição POST para /api/pedidos
            if (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                context.Request.Path.StartsWithSegments("/api/pedidos"))
            {
                var horaAtual = DateTime.Now.Hour;

                // Se estiver fora do horário permitido
                if (horaAtual < _horaInicio || horaAtual >= _horaFim)
                {
                    context.Response.StatusCode = StatusCodes.Status423Locked; // ou Status403Forbidden
                    await context.Response.WriteAsync(
                        $"Só aceitamos pedidos das {_horaInicio}h às {_horaFim}h. " +
                        $"Horário atual: {horaAtual}h."
                    );
                    return; // Interrompe o pipeline
                }
            }

            // Se estiver dentro do horário permitido, segue o fluxo
            await _next(context);
        }
    }
}
