using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    // FILTRO DE RESULTADO (IResultFilter):
    // Um filtro de resultado é executado antes e depois de a resposta ser enviada ao cliente.
    // Ele permite que você modifique ou "envolva" (wrap) o resultado de qualquer endpoint.
    //
    // Para que serve este filtro?
    // Sem este filtro, cada endpoint retorna o dado "cru":
    //   { "id": 1, "nome": "Maria" }
    //
    // Com este filtro, TODAS as respostas ficam padronizadas em um "envelope":
    //   { "sucesso": true, "dados": { "id": 1, "nome": "Maria" }, "mensagem": "Operação realizada com sucesso" }
    //
    // Isso facilita para o front-end (aplicativo/site) tratar as respostas de forma uniforme.
    public class EnvolveRespostaFilter : IResultFilter
    {
        // OnResultExecuted: executado DEPOIS que a resposta foi enviada ao cliente.
        // Aqui apenas registramos que o filtro passou por esta etapa.
        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("Filtro de resultado executado");
        }

        // OnResultExecuting: executado ANTES de enviar a resposta ao cliente.
        // Aqui é onde fazemos o trabalho de "embalar" a resposta no envelope padrão.
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // 1. Obtém o código de status HTTP da resposta atual.
            // Ex: 200 (OK), 201 (Created), 400 (BadRequest), 404 (NotFound), etc.
            int statusCode = context.HttpContext.Response.StatusCode;

            // Respostas com código entre 200 e 299 são consideradas sucesso.
            // Ex: 200 OK, 201 Created, 204 NoContent.
            bool sucesso = statusCode >= 200 && statusCode < 300;

            // 2. Extrai os dados reais do resultado do endpoint.
            // IActionResult é a interface genérica de retorno dos endpoints.
            // Precisamos verificar qual tipo específico foi retornado para extrair o valor.
            object dadosPuros = null;

            // Se o resultado é um ObjectResult (retornado por Ok(), BadRequest(), etc.),
            // extraimos o valor dentro dele.
            if (context.Result is ObjectResult objectResult)
            {
                dadosPuros = objectResult.Value;
            }
            // Se o resultado é um JsonResult (formato JSON explícito),
            // extraimos o valor de dentro.
            else if (context.Result is JsonResult jsonResult)
            {
                dadosPuros = jsonResult.Value;
            }
            // Se for outro tipo (ex: EmptyResult), dadosPuros continua null.

            // 3. Monta o objeto "envelope" que padroniza a resposta.
            // Usando "new { }" criamos um objeto anônimo com as propriedades desejadas.
            var envelope = new
            {
                sucesso = sucesso,
                dados = dadosPuros,   // Os dados reais do endpoint
                mensagem = sucesso ? "Operação realizada com sucesso" : "Operação realizada com falha"
            };

            // 4. Substitui o resultado original pelo envelope padronizado.
            // O StatusCode é mantido igual ao original para não alterar o comportamento HTTP.
            context.Result = new ObjectResult(envelope)
            {
                StatusCode = statusCode
            };
        }
    }
}
