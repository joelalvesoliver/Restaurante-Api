using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api
{
    public class ArgumentExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ArgumentException exception)
            {
                // Monta a resposta JSON
                var resposta = new
                {
                    sucesso = false,
                    erro = exception.Message,
                    tipo = nameof(ArgumentException)
                };

                //Define o resultado como 422 UnprocessableEntity
                context.Result = new ObjectResult(resposta)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };

                // Marca a exceção como tratada
                context.ExceptionHandled = true;
            }
        }
    }

}
