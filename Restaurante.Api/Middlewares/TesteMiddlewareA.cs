namespace Restaurante.Api.Middlewares
{
    public class TesteMiddlewareA
    {
        public class MiddlewareA
        {
            private readonly RequestDelegate _next;

            public MiddlewareA(RequestDelegate next) => _next = next;

            public async Task InvokeAsync(HttpContext context)
            {
                Console.WriteLine("➡️ [A] Entrada");
                await _next(context);
                Console.WriteLine("⬅️ [A] Saída");
            }
        }

    }
}
