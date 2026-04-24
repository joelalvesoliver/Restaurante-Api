using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    // FILTRO DE EXCEÇÃO (IExceptionFilter):
    // Em programação, uma "Exception" (exceção) é um erro que ocorre durante a execução.
    // Se não tratado, o erro derruba a requisição e retorna um erro genérico ao cliente.
    //
    // Este filtro intercepta exceções que ocorrem em qualquer endpoint da API e
    // transforma o erro em uma resposta HTTP amigável e padronizada.
    //
    // Para que serve?
    // Em vez de o cliente receber uma página de erro do servidor (500 Internal Server Error
    // com stack trace exibido), ele recebe uma mensagem clara e controlada.
    // Além disso, esconde detalhes técnicos internos que não devem ser expostos.
    public class ExceptionFilter : IExceptionFilter
    {
        // OnException: executado automaticamente sempre que uma exceção não tratada ocorre
        // dentro de um endpoint protegido por este filtro.
        public void OnException(ExceptionContext context)
        {
            // Verifica se a exceção lançada é especificamente do tipo DomainException.
            // "DomainException" representa erros de regra de negócio (regras do domínio).
            // Ex: "Um garçom não pode criar outro garçom."
            // A palavra "is" verifica o tipo E já faz o cast (conversão) automaticamente.
            if(context.Exception is DomainException exception)
            {
                // Monta um objeto de resposta padronizado para erros de domínio
                var resposta = new
                {
                    sucesso = false,
                    erro = exception.Message,          // Mensagem de erro da exceção
                    tipo = exception.InnerException    // Exceção interna (causa original), se houver
                };

                // Define o resultado como um 400 Bad Request com a mensagem de erro.
                // BadRequestObjectResult é equivalente a retornar BadRequest(resposta) em um controller.
                context.Result = new BadRequestObjectResult(resposta);

                // Marca a exceção como "tratada" para que o .NET não tente processar o erro novamente.
                context.ExceptionHandled = true;

                return;
            }
            // Se a exceção não for uma DomainException, o filtro deixa passar
            // e o .NET trata como um erro 500 interno.
        }
    }

    // DomainException: uma exceção customizada para erros de regra de negócio.
    // Em C#, podemos criar nossos próprios tipos de exceção herdando de Exception.
    // Isso permite diferenciar erros de negócio (ex: "Pedido já foi pago")
    // de erros técnicos (ex: banco de dados fora do ar).
    // "internal" significa que esta classe só pode ser usada dentro deste projeto.
    internal class DomainException : Exception
    {
        public DomainException() { }
    }
}
