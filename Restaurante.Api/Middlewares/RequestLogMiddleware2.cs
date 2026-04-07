namespace Restaurante.Api.Middlewares
{
    public class RequestLogMiddleware2
    {
        private readonly RequestDelegate _next;

        public RequestLogMiddleware2(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = context.Request.Headers["traceId"].ToString();

            Console.WriteLine($"[Init Log2] {context.Request.Method}" +
                $" {context.Request.Path} {traceId}");

            await _next(context);

            Console.WriteLine($"[end Log2]  {context.Response.StatusCode}" +
                $"{traceId}");
        }
    }
}
