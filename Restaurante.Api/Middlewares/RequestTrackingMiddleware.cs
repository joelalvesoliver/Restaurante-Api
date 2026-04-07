namespace Restaurante.Api.Middlewares
{
    public class RequestTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = Guid.NewGuid().ToString();
            context.Request.Headers["traceId"] = traceId;

            Console.WriteLine($"[Trackin IN ] {context.Request.Method}" +
                $" {context.Request.Path} {traceId}");

            await _next(context);

            Console.WriteLine($"[Trackin OUT]  {context.Response.StatusCode}" +
                $"{traceId}");
        }
    }
}
