namespace Restaurante.Api.Middlewares
{
    // MIDDLEWARE de rastreamento de requisições.
    //
    // O que é um Middleware?
    // Middleware é um componente que fica "no meio" do caminho entre o cliente e o endpoint.
    // Pense como uma esteira de produção: cada requisição passa por cada middleware
    // em ordem, antes de chegar ao controller.
    //
    // O .NET permite encadear vários middlewares em sequência — isso é chamado de
    // "pipeline" de requisições. A ordem de registro no Program.cs importa muito!
    //
    // Para que serve ESTE middleware?
    // Toda requisição que entra na API recebe um IDENTIFICADOR Único (TraceId/UUID).
    // Este Id permite rastrear a requisição do início ao fim nos logs,
    // o que facilita muito encontrar erros em produção.
    // Ex: se um cliente reporta um problema, você busca o traceId nos logs para ver
    // exatamente o que aconteceu com aquela requisição específica.
    public class RequestTrackingMiddleware
    {
        // RequestDelegate representa o próximo middleware (ou endpoint) na sequência.
        // Ao chamar _next(context), passamos a requisição adiante no pipeline.
        private readonly RequestDelegate _next;

        // Construtor: recebe o próximo middleware via Injeção de Dependência.
        // O .NET conecta automaticamente os middlewares em ordem.
        public RequestTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // InvokeAsync: método executado para CADA requisição que passa por este middleware.
        // "async" e "await" indicam operação assíncrona: o servidor não fica bloqueado
        // esperando; ele pode atender outras requisições enquanto esta é processada.
        public async Task InvokeAsync(HttpContext context)
        {
            // Gera um GUID (Globally Unique Identifier) — um identificador universalmente único.
            // Exemplo de GUID: "a3f2c1d0-4e5b-11ed-b878-0242ac120002"
            // A probabilidade de dois GUIDs iguais é astronomicamente pequena.
            var traceId = Guid.NewGuid().ToString();

            // Insere o traceId no cabeçalho da requisição para que outros middlewares
            // e filtros possam acessar este Id para logs e rastreamento.
            context.Request.Headers["traceId"] = traceId;

            // Registra no console a entrada da requisição:
            // Método HTTP (GET, POST, etc.), caminho da URL e o traceId gerado.
            Console.WriteLine($"[Trackin IN ] {context.Request.Method}" +
                $" {context.Request.Path} {traceId}");

            // Passa a requisição para o próximo middleware no pipeline.
            // Tudo que vem DEPOIS desta linha é executado na VOLTA (após o endpoint responder).
            await _next(context);

            // Registra no console a saída da resposta:
            // Código de status HTTP (200, 404, 500, etc.) e o traceId para correlacionar.
            Console.WriteLine($"[Trackin OUT]  {context.Response.StatusCode}" +
                $"{traceId}");
        }
    }
}
