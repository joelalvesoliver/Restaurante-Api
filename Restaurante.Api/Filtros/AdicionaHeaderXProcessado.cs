using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;



namespace Restaurante.Api.Filtros
{
    public class AdicionaHeaderXProcessado : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("Filtro de resultado executado");

        }

            public void OnResultExecuting(ResultExecutingContext context)
            {

                var horaAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                context.HttpContext.Response.Headers["X-Processado"] = horaAtual;

                Console.WriteLine("Filtro de resultado exercicio executando");



            }



    }
}





