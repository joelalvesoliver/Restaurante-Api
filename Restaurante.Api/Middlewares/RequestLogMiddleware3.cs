namespace Restaurante.Api.Middlewares
{
    public class RequestLogMiddleware3
    {
        private readonly RequestDelegate _next;

        public RequestLogMiddleware3(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = context.Request.Headers["traceId"].ToString();

            Console.WriteLine($"[Init Log3] {context.Request.Method}" +
                $" {context.Request.Path} {traceId}");

            await _next(context);

            Console.WriteLine($"[end Log3]  {context.Response.StatusCode}" +
                $"{traceId}");
        }
    }
}
