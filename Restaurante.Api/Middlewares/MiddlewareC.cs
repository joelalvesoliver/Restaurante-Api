namespace Restaurante.Api.Middlewares
{
    public class MiddlewareC
    {
        private readonly RequestDelegate _next;

        public MiddlewareC(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("Entrando no Middleware C");
            await _next(context);
            Console.WriteLine("Saindo do Middleware C");
        }
    }
}
