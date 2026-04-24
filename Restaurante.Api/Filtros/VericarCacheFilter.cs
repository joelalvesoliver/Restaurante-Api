using Microsoft.AspNetCore.Mvc.Filters;

namespace Restaurante.Api.Filtros
{
    // FILTRO DE RECURSO (IResourceFilter):
    // É executado muito cedo no pipeline de requisições, antes mesmo do model binding
    // (processo de converter o JSON da requisição em objetos C#).
    // Por isso, é ideal para verificar caches, pois podemos responder rapidamente
    // sem precisar passar por todas as etapas de processamento.
    //
    // O que é CACHE?
    // Cache é uma "memória rápida" que guarda resultados já processados.
    // Em vez de buscar os dados no banco toda vez, o cache retorna o resultado salvo
    // de uma busca anterior. Isso acelera a API e reduz a carga no banco de dados.
    //
    // Exemplo prático:
    // 1ª requisição: busca todos os pratos no banco (lento) → salva no cache.
    // 2ª requisição: retorna do cache diretamente (muito rápido) → pula o banco.
    public class VericarCacheFilter : IResourceFilter
    {
        // OnResourceExecuted: executado DEPOIS que o recurso (endpoint) foi processado.
        // Aqui é onde atualizaríamos o cache com o novo resultado obtido.
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // Este ponto seria usado para salvar o novo resultado no cache
            Console.WriteLine("Atualiza o cache");
        }

        // OnResourceExecuting: executado ANTES de processar o recurso.
        // Aqui verificamos se já existe um resultado em cache para esta requisição.
        // Se existir, retornamos o cache e interrompemos o processamento (short-circuit).
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // Este ponto seria usado para verificar se o resultado já está em cache.
            Console.WriteLine("Verificar no cache");
            // Aqui manipulamos o cabeçalho da requisição para controle de cache.
            // Em um cenário real, você verificaria um sistema de cache como Redis.
            context.HttpContext.Request.Headers["cache"] = "";
        }
    }
}
