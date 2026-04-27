using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filters
{
    public class ArgumentExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ArgumentException ex)
            {
                var resposta = new
                {
                    sucesso = false,
                    erro = ex.Message,
                    tipo = "ArgumentException"
                };

                context.Result = new ObjectResult(resposta)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };

                context.ExceptionHandled = true;
            }
        }
    }
}   