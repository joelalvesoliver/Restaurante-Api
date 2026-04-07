namespace Restaurante.Api.Middlewares
{
    public class RequestLogMiddleware1
    {
        private readonly RequestDelegate _next;

        public RequestLogMiddleware1(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = Guid.NewGuid().ToString();
            context.Request.Headers["traceId"] = traceId;

            Console.WriteLine($"[Init Log1] {context.Request.Method}" +
                $" {context.Request.Path} {traceId}");

            await _next(context);

            Console.WriteLine($"[end Log1]  {context.Response.StatusCode}" +
                $"{traceId}");
        }
    }
}
