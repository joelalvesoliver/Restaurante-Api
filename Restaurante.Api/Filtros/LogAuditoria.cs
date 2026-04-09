using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    public class LogAuditoria : IActionFilter   
    {
        private readonly ILogger<LogAuditoria> _logger;
        public LogAuditoria(ILogger<LogAuditoria> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // logica que será executada após acionar a rota
            var trackingId = context.HttpContext.Request.Headers["traceId"];
            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            var usuario = context.HttpContext.User?.Identity?.Name ?? "Anonimo";

            Console.WriteLine($"[LogAuditoria][OnActionExecuted] {controller} {action} {usuario}" +
                $" {trackingId}");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // logica que será executada antes de chamada da rota
            var trackingId = context.HttpContext.Request.Headers["traceId"];
            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            var usuario = context.HttpContext.User?.Identity?.Name ?? "Anonimo";
            
            Console.WriteLine($"[LogAuditoria][OnActionExecuting] {controller} {action} {usuario}" +
                $" {trackingId}");

        }
    }
}
