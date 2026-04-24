using System.Diagnostics;

namespace Restaurante.Api.Middlewares
{
    // MIDDLEWARE de bloqueio por cabeçalho (Header).
    //
    // O que são cabeçalhos HTTP (Headers)?
    // Além do corpo da requisição (body), as requisições HTTP também carregam
    // "cabeçalhos" — metadados sobre a requisição.
    // Ex: tipo de conteúdo, autenticação, idioma, canal de origem...
    //
    // Para que serve ESTE middleware?
    // Controla o acesso à rota "/api/Cardapio" com base no cabeçalho "X-Canal".
    // Só requisições com X-Canal = "app" são permitidas.
    // Requisições de outros canais (ex: browser direto, postman sem o cabeçalho) são bloqueadas.
    //
    // Isso é útil para garantir que apenas o aplicativo móvel oficial
    // (que envia o cabeçalho correto) possa acessar determinadas rotas.
    public class BloqueioHeaderMiddleware
    {
        // Referência para o próximo middleware no pipeline.
        private readonly RequestDelegate _next;

        // Construtor: recebe o próximo middleware automaticamente.
        public BloqueioHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // InvokeAsync: executado para cada requisição que passa por este middleware.
        public async Task InvokeAsync(HttpContext context)
        {
            // Verifica se a rota da requisição começa com "/api/Cardapio".
            // Se for uma rota diferente, este middleware não faz nada (pula a verificação).
            if (context.Request.Path.StartsWithSegments("/api/Cardapio"))
            {
                // Lê o valor do cabeçalho "X-Canal" da requisição.
                // Cabeçalhos com "X-" são customizados (não-padrão HTTP).
                var canal = context.Request.Headers["X-Canal"].ToString();

                // Verifica se o canal NÃO é "app" (ignora maiúsculas/minúsculas).
                if(!string.Equals(canal, "app", StringComparison.OrdinalIgnoreCase))
                {
                    // Lê o traceId inserido pelo RequestTrackingMiddleware para o log.
                    var traceId = context.Request.Headers["traceId"].ToString();
                    Console.WriteLine($"[Trackin] Requisicao invalida, canal {canal} não suportado, traceId {traceId}");

                    // Define o status da resposta como 403 Forbidden (acesso proibido).
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    // "return" sem chamar _next() interrompe o pipeline aqui.
                    // O endpoint nunca chega a ser executado. Isso se chama "short-circuit".
                    return;
                }
            }

            // Se não for rota do cardápio, ou se o canal for "app", passa adiante no pipeline.
            await _next(context);
        }
    }
}
