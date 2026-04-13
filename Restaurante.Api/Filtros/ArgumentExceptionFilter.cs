using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    public class ArgumentExceptionFilter : IExceptionFilter
    {
        /* 
        * Aula 03 - Exercício 3
        */
        public void OnException(ExceptionContext context)
        {
            if(context.Exception is ArgumentException exception)
            {
                var resposta = new
                {
                    sucesso = false,
                    erro = exception.Message,
                    tipo = "ArgumentException"
                };

                context.Result = new UnprocessableEntityObjectResult(resposta);
                context.ExceptionHandled = true;
                return;
            }
        }
    }
}
