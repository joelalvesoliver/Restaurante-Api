using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    // FILTRO DE AÇÃO (IActionFilter):
    // Filtros de ação são executados imediatamente antes e depois do método do controller (endpoint).
    // São ideais para tarefas transversais (cross-cutting concerns), como:
    //   - Auditoria (registrar quem fez o quê e quando)
    //   - Validações extras antes de processar a requisição
    //   - Medição de tempo de execução
    //
    // Este filtro específico serve para AUDITORIA:
    // registra no log informações sobre quem chamou qual endpoint.
    // Auditoria é fundamental em sistemas corporativos para rastrear ações dos usuários.
    public class LogAuditoria : IActionFilter
    {
        // Logger para registrar eventos. Injetado via Injeção de Dependência.
        private readonly ILogger<LogAuditoria> _logger;

        // Construtor: recebe o logger automaticamente via Injeção de Dependência.
        // O .NET sabe qual logger criar porque registramos LogAuditoria com AddScoped no Program.cs.
        public LogAuditoria(ILogger<LogAuditoria> logger)
        {
            _logger = logger;
        }

        // OnActionExecuted: executado DEPOIS do endpoint ser chamado.
        // Registra no log as informações da requisição que já foi processada.
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Obtém o traceId (identificador da requisição) do cabeçalho HTTP.
            // Este Id foi inserido pelo RequestTrackingMiddleware para rastrear a requisição.
            var trackingId = context.HttpContext.Request.Headers["traceId"];

            // Obtém o nome do controller e do action (método) que foi executado.
            // Ex: controller = "Cardapio", action = "Index"
            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];

            // Tenta obter o nome do usuário autenticado.
            // Se o usuário não estiver logado, usa "Anonimo" como padrão.
            // O operador "??" é chamado de "null-coalescing": retorna o lado direito se o esquerdo for null.
            var usuario = context.HttpContext.User?.Identity?.Name ?? "Anonimo";

            Console.WriteLine($"[LogAuditoria][OnActionExecuted] {controller} {action} {usuario}" +
                $" {trackingId}");
        }

        // OnActionExecuting: executado ANTES do endpoint ser chamado.
        // Registra no log as informações da requisição que está prestes a ser processada.
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Obtém o traceId do cabeçalho da requisição.
            var trackingId = context.HttpContext.Request.Headers["traceId"];

            // Obtém o nome do controller e do action.
            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];

            // Obtém o nome do usuário autenticado, ou "Anonimo" se não houver.
            var usuario = context.HttpContext.User?.Identity?.Name ?? "Anonimo";

            Console.WriteLine($"[LogAuditoria][OnActionExecuting] {controller} {action} {usuario}" +
                $" {trackingId}");
        }
    }
}
