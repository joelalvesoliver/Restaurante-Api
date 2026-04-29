using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    public class ExceptionFilterArgument : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ArgumentException exception)
            {
                var resposta = new
                {
                    sucesso = false,
                    erro = exception.Message,
                    tipo = exception.GetType().Name
                };

                context.Result = new ObjectResult(resposta)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
                context.ExceptionHandled = true;

                return;
            }
        }





    }
}




