using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filters
{
    public class ValidarIdPositivoFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // verifica se existe um argumento chamado "id" na action
            if (context.ActionArguments.TryGetValue("id", out var valor) && valor is int id)
            {
                if (id <= 0)
                {
                    // bloqueia a action e devolve 400
                    context.Result = new BadRequestObjectResult("O parâmetro 'id' deve ser positivo.");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // opcional (não é necessário para este exercício)
        }
    }
}