using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    public class HeaderCustomizadoFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            //Código executado após  a execução da rota
            Console.WriteLine("Filtro de dataHora executado");
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            /* 
            * Aula 03 - Exercício 2
            */
            var horaExecucao = DateTime.Now; 
            Console.WriteLine("Conferir o horário no cabeçalho");
            
            //o comando ADD, lança uma exceção ArgumentException se a chave já existir, assim, para utilizá-la, 
            //teríamos que tratar a exceção através de um bloco try/catch.
            //context.HttpContext.Response.Headers.Add("X-Processado-Em", horaExecucao.ToString());
            
            //Preferi usar o Append, por que ele adiciona a chave independente de ela já existir
            context.HttpContext.Response.Headers.Append("X-Processado-Em", horaExecucao.ToString());

           //Uma outra forma seria a chamada abaixo, nesse caso, o valor será criado, caso o valor já exista, ele será alterado.
           //context.HttpContext.Response.Headers["X-Processado-Em"] = horaExecucao.ToString();
        }
    }
}
