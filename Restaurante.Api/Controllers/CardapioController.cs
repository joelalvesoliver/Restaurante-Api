using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.Filtros;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Service;

namespace Restaurante.Api.Controllers
{
    // [ApiController]: define esta classe como um controller de API REST.
    [ApiController]
    // A URL base para todos os endpoints deste controller será "api/cardapio".
    [Route("api/[controller]")]
    // [ServiceFilter]: aplica um filtro registrado no sistema de injeção de dependência.
    // Esta linha está comentada (//), então o filtro NÃO está ativo no momento.
    // Se ativada, todas as rotas deste controller passariam pelo VerificarAutorizacaoFazerPedido.
    //[ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
    public class CardapioController : ControllerBase
    {
        // Construtor vazio: este controller não tem dependências injetadas por enquanto.
        // Em um sistema mais completo, receberíamos o repositório de pratos aqui.
        public CardapioController() 
        {           
        }
        
        // GET api/cardapio
        // Retorna a lista de todos os pratos do cardápio.
        //
        // [HttpGet("")]: responde a GET na URL base do controller (sem sufixo).
        // [ServiceFilter(typeof(LogAuditoria))]: aplica o filtro de auditoria APENAS neste endpoint.
        // Toda vez que alguém listar o cardápio, o filtro LogAuditoria vai registrar isso.
        [HttpGet("")]
        [ServiceFilter(typeof(LogAuditoria))]
        public IActionResult Index()
        {
            // IActionResult é a interface base de retorno dos endpoints.
            // Content() retorna uma resposta com texto simples (200 OK).
            return Content("Lista de pratos do cardapio");
        }

        // GET api/cardapio/prato/{id}
        // Retorna os detalhes de um prato específico pelo seu Id.
        //
        // {id} é um parâmetro de rota: parte variável da URL capturada automaticamente.
        // Ex: GET /api/cardapio/prato/5 → id = 5
        //
        // [ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]: aplica o filtro de autorização
        // APENAS neste endpoint. Demonstra que filtros podem ser aplicados por rota individualmente.
        [HttpGet("prato/{id}")]
        [ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
        public IActionResult ObterPratoPeloId(int id)
        {
            return Content($"Detalhes do prato de id {id}");
        }

        // As rotas abaixo estão comentadas como exemplos de outros padrões de URL:

        // Exemplo de rota com GUID (identificador único global):
        // Um GUID é mais seguro que um int sequencial, pois não revela quantos registros existem.
        //[HttpGet("prato-guid/{id:guid}")]
        //public IActionResult ObterPratoPorGuid(Guid id)
        //{
        //    return Content($"Busca do prato com o guid {id}");
        //}

        // Exemplo de rota com múltiplos segmentos e filtro de cache:
        // Demonstra como criar URLs descritivas com vários parâmetros.
        // api/cardapio/categoria/saladas/pagina/1
        //[HttpGet("categoria/{nomeCategoria}/pagina/{pagina:int}")]
        //[TypeFilter(typeof(VericarCacheFilter))]
        //public IActionResult ListarPorCategoria(string nomeCategoria, int pagina)
        //{
        //    return Content($"Categoria {nomeCategoria}, pagina {pagina}");
        //}
    }
}


/*
 OBS: Palavras reservadas, evitar usa - las na construção das rotas
 action
 controller
 area
 page
 handler

 
200 OK: leitura ou operacao concluida com retorno;
201 Created: recurso criado com sucesso;
204 NoContent: sucesso sem corpo de resposta;

400 BadRequest: erro de validacao/entrada;
401 Unauthorized: nao autenticado;
403 Forbidden: autenticado, mas sem permissao;
404 NotFound: recurso nao encontrado;

500 InternalServerError: erro inesperado no servidor.
503 Serviço indiponível
504 timeout, demorou para responder
 
 */