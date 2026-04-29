using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Restaurante.Api.DTOs;
using Restaurante.Api.Filtros;
using Restaurante.Api.Services;
using SimuladorBancoDados;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Entidade;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
    public class CardapioController : ControllerBase
    {
        private readonly IPratoRepository _repository;
        private readonly Services.ArquivoService _arquivoService;
        private readonly ILogger<CardapioController> _logger;
        public CardapioController(
            IPratoRepository repository,
            ArquivoService arquivoService,
            ILogger<CardapioController> logger)
        {
            _repository = repository;
            _arquivoService = arquivoService;
            _logger = logger;
        }

        //api/cardapio
        [HttpGet("")]
        [ServiceFilter(typeof(LogAuditoria))]
        [ProducesResponseType(typeof(IEnumerable<PratoRespostaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult ListarTodos()
        {
            _logger.LogInformation("Iniciando busca de todos os pratos.");

            var pratos = _repository.BuscaTodosPratos();

            if (pratos == null || !pratos.Any())
            {
                _logger.LogWarning("Nenhum prato encontrado.");
                return NoContent();
            }

            var response = pratos.Select(p => new PratoRespostaDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Preco = p.Preco,
                Descricao = p.Descricao,
                Categoria = p.Categoria,
                DataCadastro = p.DataCadastro,
                IdFoto = p.IdFoto,

                UrlDownloadFoto = $"{Request.Scheme}://{Request.Host}/api/arquivos/download/{p.IdFoto}"
            });

            _logger.LogInformation("Consulta finalizada com {Count} pratos encontrados.", pratos.Count());

            return Ok(response);
        }


        [HttpGet("prato/{id:int}")]
        [ProducesResponseType(typeof(PratoRespostaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ObterPorId(int id)
        {

            if (id <= 0)
            {
                _logger.LogWarning("Tentativa de busca com ID inválido: {Id}", id);
                return BadRequest("O ID do prato deve ser maior que zero.");
            }

            _logger.LogInformation("Buscando detalhes do prato ID: {Id}", id);

            var prato = _repository.BuscaPratoPeloId(id);

            if (prato == null)
            {
                _logger.LogWarning("Prato com ID {Id} não foi encontrado.", id);
                return NotFound($"Prato com ID {id} não encontrado.");
            }


            var response = new PratoRespostaDTO
            {
                Id = prato.Id,
                Nome = prato.Nome,
                Preco = prato.Preco,
                Descricao = prato.Descricao,
                Categoria = prato.Categoria,
                Ativo = prato.Ativo,
                DataCadastro = prato.DataCadastro,
                IdFoto = prato.IdFoto,
                UrlDownloadFoto = $"{Request.Scheme}://{Request.Host}/api/arquivos/download/{prato.IdFoto}"
            };

            return Ok(response);
        }


        [HttpPost]
        [ProducesResponseType(typeof(PratoRespostaDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Cadastrar([FromBody] CreatePratoDTO novoPratoDto)
        {
            _logger.LogInformation("Iniciando cadastro de novo prato: {Nome}", novoPratoDto.Nome);

            try
            {
                

                var prato = new SimuladorBancoDados.Entidade.Prato
                {
                    Nome = novoPratoDto.Nome,
                    Preco = novoPratoDto.Preco,
                    Descricao = novoPratoDto.Descricao,
                    Categoria = novoPratoDto.Categoria,
                    IdFoto = novoPratoDto.IdFoto,
                    Ativo = true,
                    DataCadastro = DateTime.Now
                };

                
                _repository.AdicionaPrato(prato);

                _logger.LogInformation("Prato cadastrado com sucesso! ID gerado: {Id}", prato.Id);

                var response = new PratoRespostaDTO
                {
                    Id = prato.Id,
                    Nome = prato.Nome,
                    Preco = prato.Preco,
                    Descricao = prato.Descricao,
                    Categoria = prato.Categoria,
                    Ativo = prato.Ativo,
                    DataCadastro = prato.DataCadastro,
                    IdFoto = prato.IdFoto,
                    UrlDownloadFoto = $"{Request.Scheme}://{Request.Host}/api/arquivos/download/{prato.IdFoto}"
                };

                return CreatedAtAction(nameof(ObterPorId), new { id = prato.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar o prato {Nome}", novoPratoDto.Nome);
                return StatusCode(500, "Erro interno ao processar o cadastro.");
            }
        }

    }

}
        //api/cardapio/prato/{id}

        /*[HttpGet("prato/{id}")]
        [ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
        public IActionResult ObterPratoPeloId(int id)
        {
            return Content($"Detalhes do prato de id {id}");
        }*/



    

        



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