using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.Filtros;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Service;
using Restaurante.Api.DTOs;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
    public class CardapioController : ControllerBase
    {

        // Ferramentas que a classe vai usar
        private readonly IPratoRepository _repository;
        private readonly ILogger<CardapioController> _logger;

        public CardapioController(IPratoRepository repository, ILogger<CardapioController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Construtor: aqui o sistema "entrega" as ferramentas para o Controlle
   
        //api/cardapio

        [HttpGet("")]
        [ServiceFilter(typeof(LogAuditoria))]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult ListarTodos()
        {
            _logger.LogInformation("Buscando todos os pratos.");
            var pratos = _repository.BuscaTodosPratos();
            if (pratos == null || !pratos.Any())
                return NoContent();

            var listaDto = pratos.Select(p => new PratoResponseDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Preco = p.Preco,

                Descricao = p.Descricao,
                Categoria = p.Categoria,
                IdFoto = p.IdFoto,
                Ur1Download = $"/api/cardapio/download-foto/{p.IdFoto}",
                DataCadastro = p.DataCadastro,
                Ativo = p.Ativo,

            }).ToList();

            return
            Ok(listaDto);
        }

        [HttpPost]
        public IActionResult
            CadastrarPrato([FromBody]
            CreatePratoDTO novoPrato)
        {
            _logger.LogInformation("Cadastrando novo prato:{Nome}", novoPrato.Nome);

            // Aqui o código vai continuar no futuro
            // Por enquanto, só preciso que ele retorne algo

            return CreatedAtAction(nameof(ListarTodos), novoPrato);
        }

        //api/cardapio/prato/{id}

        [HttpGet("prato/{id}")]
        [ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
        public IActionResult ObterPratoPeloId(int id)

        {
            _logger.LogInformation($"Buscando prato com ID: {id}");
            var prato = _repository.BuscaPratoPeloId(id);
            if (prato == null)
            {

                _logger.LogWarning($"Prato{id} não encontrado.");

                return NotFound();
            }

            return Ok(new PratoResponseDTO
            {

                Id = prato.Id,
                Nome = prato.Nome,
                Preco = prato.Preco,
                Descricao = prato.Descricao,
                Categoria = prato.Categoria,
                DataCadastro = prato.DataCadastro,
                IdFoto = prato.IdFoto,
                Ativo = prato.Ativo,
                Ur1Download = $"/api/cardapio/dowload-foto/{prato.IdFoto}"
            });
        }                                                                  
        
    //3f2504e0-4f89-11d3-9a0c-0305e82c3301
    //[HttpGet("prato-guid/{id:guid}")]
    //public IActionResult ObterPratoPorGuid(Guid id)
    //{
    //    return Content($"Busca do prato com o guid {id}");
    //}
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