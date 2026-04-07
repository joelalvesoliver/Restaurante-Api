namespace Restaurante.Api.Middlewares
{
    public class MiddlewareA
    {
        private readonly RequestDelegate _next;

        public MiddlewareA(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("Entrando no Middleware A");
            await _next(context);
            Console.WriteLine("Saindo do Middleware A");
        }
    }
}
