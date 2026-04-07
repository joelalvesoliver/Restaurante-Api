namespace Restaurante.Api.Middlewares
{
    public class CabecalhoObrigatorioMiddleware
    {
        private readonly RequestDelegate _next;

        public CabecalhoObrigatorioMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                string turma = context.Request.Headers["X-turma"].ToString();
                if(turma.Length < 1)
                {
                    var traceId = context.Request.Headers["traceId"].ToString();
                    Console.WriteLine($" Requisição {traceId} mal formatada!");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }                
            }
            await _next(context);
        }
    }
}
