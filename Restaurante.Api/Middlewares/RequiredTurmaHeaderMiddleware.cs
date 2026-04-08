namespace Restaurante.Api.Middlewares
{
    public class RequiredTurmaHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public RequiredTurmaHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Só aplica a regra para rotas que começam com /api
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                if (!context.Request.Headers.ContainsKey("X-Turma"))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Cabeçalho obrigatório 'X-Turma' não encontrado.");
                    return; // Interrompe o pipeline
                }
            }

            // Se o cabeçalho existir (ou não for rota /api), segue normalmente
            await _next(context);
        }
    }
}
