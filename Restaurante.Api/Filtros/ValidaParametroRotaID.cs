using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.Eventing.Reader;
namespace Restaurante.Api.Filtros
{
    public class ValidaParametroRotaID : IActionFilter
    {

        


        private readonly ILogger<ValidaParametroRotaID> _logger;
        public ValidaParametroRotaID(ILogger<ValidaParametroRotaID> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            if (!context.ActionArguments.TryGetValue("id", out var valor)) //verifica se id existe em ActionArguments
                return;
           

            if (!(valor is int id) || id<=0) //verifica se é inteiro e não positivo
            {


                var retorno = new
                {
                    erro = "ID inválido",
                    mensagem = "O ID deve ser um número inteiro positivo."
                };


                

                context.Result = new BadRequestObjectResult(retorno);
                    return;
            }




        }
    }
}






