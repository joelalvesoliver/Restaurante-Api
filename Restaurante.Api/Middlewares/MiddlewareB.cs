namespace Restaurante.Api.Middlewares
{
    public class MiddlewareB
    {
        private readonly RequestDelegate _next;

        public MiddlewareB(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("=> [B] Entrada");
            await _next(context);
            Console.WriteLine("<= [B] Saída");
        }
    }
}
