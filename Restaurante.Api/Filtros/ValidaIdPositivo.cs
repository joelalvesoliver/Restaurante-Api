using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    public class ValidaIdPositivo : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // não é necessário
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var id = (int)context.ActionArguments["id"];

            if (id <= 0)
            {
                // Se o id for negativo ou zero, retorna 400 (BadRequest)
                context.Result = new BadRequestObjectResult("O ID deve ser positivo.");

            }
        }
    }
}
