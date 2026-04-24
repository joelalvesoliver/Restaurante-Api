using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Restaurante.Api.Filtros
{
    // FILTRO DE AUTORIZAÇÃO (IAuthorizationFilter):
    // É o primeiro filtro a ser executado no pipeline, ANTES de qualquer outro.
    // Serve para verificar se o usuário tem PERMISSÃO para acessar determinado recurso.
    //
    // Diferença entre Autenticação e Autorização (conceito importante!):
    //   - AUTENTICAÇÃO: "Quem é você?" → verifica a identidade (login, JWT token).
    //   - AUTORIZAÇÃO: "Você PODE fazer isso?" → verifica se tem permissão (cargo, roles).
    //
    // Exemplo: um garçom está autenticado, mas não pode cancelar pedidos (não autorizado).
    //
    // Este filtro implementa lógica de autorização customizada para verificar
    // se o usuário pode realizar pedidos. O [ServiceFilter] no controller
    // indica onde este filtro é aplicado.
    public class VerificarAutorizacaoFazerPedido : IAuthorizationFilter
    {
        // Construtor vazio — nenhuma dependência necessária por enquanto.
        // Mas por estar registrado no Program.cs com AddScoped, o .NET sabe como criá-lo.
        public VerificarAutorizacaoFazerPedido()
        {
        }

        // OnAuthorization: executado automaticamente ANTES do endpoint ser chamado,
        // toda vez que o filtro estiver aplicado a uma rota.
        // É aqui que a lógica de autorização deve ser implementada.
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Obtém as informações do usuário autenticado (se houver).
            // context.HttpContext.User contém os dados do JWT token, como nome e roles.
            var user = context.HttpContext.User;

            Console.WriteLine("Passamos pelo filtro de autorização");

            // Variável que controla se o acesso será negado.
            // Atualmente está como false (acesso liberado) para fins de demonstração.
            // Em um sistema real, aqui você verificaria o cargo do usuário, o horário,
            // o status da mesa, etc.
            var acessoNegado = false;

            // Se o acesso for negado, montamos a resposta de proibição (403 Forbidden).
            if (acessoNegado)
            {
                var retorno = new
                {
                    erro = "Acesso negado",
                    mensagem = "Você não tem acesso devido!"
                };

                // Ao definir context.Result, o filtro interrompe o processamento.
                // O endpoint não é chamado — o cliente recebe apenas esta resposta.
                // Status 403 Forbidden = "autenticado, mas sem permissão".
                context.Result = new ObjectResult(retorno)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
