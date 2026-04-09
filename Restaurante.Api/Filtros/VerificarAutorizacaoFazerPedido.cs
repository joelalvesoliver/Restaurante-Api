using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Restaurante.Api.Filtros
{
    public class VerificarAutorizacaoFazerPedido : IAuthorizationFilter
    {
        public VerificarAutorizacaoFazerPedido()
        {
            
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // ação do filtro de autorização são implementadas aqui
            // logica para validar se o usuario tem as devidas autorizações
            // para seguir com a requisição

            var user = context.HttpContext.User;

            Console.WriteLine("Passamos pelo filtro de autorização");
            var acessoNegado = false;
            if (acessoNegado)
            {
                var retorno = new
                {
                    erro = "Acesso negado",
                    mensagem = "Você não tem acesso devido!"
                };
                context.Result = new ObjectResult(retorno)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}

