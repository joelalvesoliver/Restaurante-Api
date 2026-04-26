using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filters
{
    public class ValidatePositiveIdFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Verifica se o parâmetro "id" existe nos argumentos da action
            if (context.ActionArguments.ContainsKey("id") && context.ActionArguments["id"] is int id)
            {
                // Se o "id" for negativo ou zero, retorna um erro 400
                if (id <= 0)
                {
                    context.Result = new BadRequestObjectResult("O ID deve ser um valor positivo.");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Não é necessário implementar nada aqui para este caso
        }
    }
}