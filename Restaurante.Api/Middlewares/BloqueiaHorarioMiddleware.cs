using System.Diagnostics;


namespace Restaurante.Api.Middlewares
{
    public class BloqueiaHorarioMiddleware
    {




            private readonly RequestDelegate _next;

            public BloqueiaHorarioMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {

                var horaAtual = DateTime.Now.TimeOfDay;


                var inicio = new TimeSpan(11, 0, 0);   // 11:00
                var termino = new TimeSpan(23, 0, 0);  // 23:00



            if (horaAtual <inicio || horaAtual > termino)
                    {
          
                        Console.WriteLine($"Fora do horário permitido que é de {inicio} a {termino}");
                      

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Fora do horário permitido. Permitido entre {inicio} e {termino}");
                return;
                    }






            Console.WriteLine("Log 1 Entrada"); //Deve ser o primeiro a ser exibido na entrada
            await _next(context);


            //Exercicio 3
            Console.WriteLine("Log 1 Saida"); //Deve ser o terceiro a ser exibido na saída pois o middleware funcionara como uma pilha, ou seja, o código após o await _next(context) só será executado depois que os middlewares seguintes forem processados e a resposta estiver sendo enviada de volta para o cliente.

        }
        }
    
}





