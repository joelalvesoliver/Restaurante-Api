using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if(context.Exception is DomainException exception)
            {
                var resposta = new
                {
                    sucesso = false,
                    erro = exception.Message,
                    tipo = exception.InnerException
                };

                context.Result = new BadRequestObjectResult(resposta);
                context.ExceptionHandled = true;

                return;
            }
        }
    }

    internal class DomainException : Exception
    {
        public DomainException() { }
    }
}
