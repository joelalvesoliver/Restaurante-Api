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
            Console.WriteLine("Entrando no Middleware B");
            await _next(context);
            Console.WriteLine("Saindo do Middleware B");
        }
    }

}
