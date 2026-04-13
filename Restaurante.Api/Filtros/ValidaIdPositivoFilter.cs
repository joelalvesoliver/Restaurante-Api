using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    public class ValidaIdPositivoFilter : IActionFilter   
    {
        public ValidaIdPositivoFilter()
        {
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Código executado após o acionamento da rota
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Código executado antes da execução da rota
            /* 
            * Aula 03 - Exercício 1
            */
            
            var argumentId = Convert.ToInt32(context.ActionArguments["Id"]);            
            if((int)argumentId < 0)
            {
                var retorno = new
                {
                    erro = "Id Negativo",
                    mensagem = "Seu ID tem que ser um número positivo!"
                };
                context.Result = new ObjectResult(retorno)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
                
            }

        }
    }
}
