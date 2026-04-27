using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.DTOs;
using Restaurante.Api.Filtros;
using Restaurante.Api.Services;
using SimuladorBancoDados.Entidade;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Service;
//using Restaurante.Api.DTOs;
//using Restaurante.Api.Services;
//using SimuladorBancoDados.Entidade;
//using SimuladorBancoDados.Interfaces;
//using SimuladorBancoDados.Service;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
    public class CardapioController : ControllerBase
    {
        private readonly ILogger<CardapioController> _logger;
        private readonly IPratoRepository _pratoRepository;
        private readonly ArquivoService _arquivoService;

        public CardapioController(ILogger<CardapioController> logger,
            IPratoRepository pratoRepository,
            ArquivoService arquivoService) 
        {
            _logger = logger;
            _pratoRepository = pratoRepository;
            _arquivoService = arquivoService;
        }

        //api/cardapio
        //[HttpGet("")]
        //[ServiceFilter(typeof(LogAuditoria))]
        //public IActionResult Index()
        //{
        //    return Content("Lista de pratos do cardapio");
        //}

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PratoResponseDTO>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult GetCardapio()
        {
            try
            {
                _logger.LogInformation("Requisição recebida para listar todos os pratos do cardápio.");

                var pratos = _pratoRepository.BuscaTodosPratos();

                if (pratos == null || !pratos.Any())
                {
                    _logger.LogInformation("Nenhum prato encontrado no cardápio.");
                    return NoContent();
                }

                var response = pratos.Select(p => new PratoResponseDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Preco = p.Preco,
                    Descricao = p.Descricao,
                    Categoria = p.Categoria,
                    Ativo = p.Ativo,
                    DataCadastro = p.DataCadastro,
                    IdFoto = p.IdFoto,
                    UrlDownload = _arquivoService.ObterUrlDownload(p.IdFoto)
                });

                _logger.LogInformation("Consulta de cardápio concluída com sucesso. Total de pratos: {Count}", response.Count());
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar cardápio.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao consultar cardápio.");
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PratoResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CadastrarPrato([FromBody] PratoCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_arquivoService.ArquivoExiste(dto.IdFoto))
                    return BadRequest($"Arquivo com Id {dto.IdFoto} não encontrado.");

                var prato = new Prato
                {
                    Nome = dto.Nome,
                    Preco = dto.Preco,
                    Descricao = dto.Descricao,
                    Categoria = dto.Categoria,
                    Ativo = dto.Ativo,
                    DataCadastro = DateTime.UtcNow,
                    IdFoto = dto.IdFoto
                };

                _pratoRepository.AdicionaPrato(prato);

                var response = new PratoResponseDTO
                {
                    Id = prato.Id,
                    Nome = prato.Nome,
                    Preco = prato.Preco,
                    Descricao = prato.Descricao,
                    Categoria = prato.Categoria,
                    Ativo = prato.Ativo,
                    DataCadastro = prato.DataCadastro,
                    IdFoto = prato.IdFoto,
                    UrlDownload = _arquivoService.ObterUrlDownload(prato.IdFoto)
                };

                return CreatedAtAction(nameof(GetPratoPorId), new { id = prato.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar prato.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao cadastrar prato.");
            }
        }


        [HttpGet("prato/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PratoResponseDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPratoPorId(int id)
        {
            try
            {
                _logger.LogInformation("Requisição recebida para obter prato com ID {Id}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("ID inválido informado: {Id}", id);
                    return BadRequest("ID inválido. O ID deve ser maior que zero.");
                }

                var prato = _pratoRepository.BuscaPratoPeloId(id);

                if (prato == null)
                {
                    _logger.LogWarning("Prato com ID {Id} não encontrado.", id);
                    return NotFound($"Prato com ID {id} não encontrado.");
                }

                var response = new PratoResponseDTO
                {
                    Id = prato.Id,
                    Nome = prato.Nome,
                    Preco = prato.Preco,
                    Descricao = prato.Descricao,
                    Categoria = prato.Categoria,
                    Ativo = prato.Ativo,
                    DataCadastro = prato.DataCadastro,
                    IdFoto = prato.IdFoto,
                    UrlDownload = _arquivoService.ObterUrlDownload(prato.IdFoto)
                };

                _logger.LogInformation("Prato {Id} retornado com sucesso.", prato.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar prato com ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao consultar prato.");
            }
        }
    }
}



//[HttpPost]
//[ProducesResponseType(StatusCodes.Status201Created)]
//[ProducesResponseType(StatusCodes.Status400BadRequest)]
//public async Task<IActionResult> CadastrarPrato([FromForm] PratoCreateDTO dto, IFormFile foto)
//{
//    var(sucesso, arquivoInfo) = await _arquivoService.UploadArquivoAsync(foto);
//    if (!sucesso)
//    {
//        return BadRequest(arquivoInfo.Mensagem);
//    }

//    var prato = new Prato
//    {
//        Nome = dto.Nome,
//        Preco = dto.Preco,
//        Descricao = dto.Descricao,
//        Categoria = dto.Categoria,
//        DataCadastro = DateTime.UtcNow,
//        IdFoto = arquivoInfo.NomeArmazenado // GUID + extensão
//    };

//    _pratoRepository.Salvar(prato);

//    var response = new PratoResponseDTO
//    {
//        Id = prato.Id,
//        Nome = prato.Nome,
//        Preco = prato.Preco,
//        Descricao = prato.Descricao,
//        Categoria = prato.Categoria,
//        DataCadastro = prato.DataCadastro,
//        IdFoto = prato.IdFoto,
//        UrlDownload = _arquivoService.ObterUrlDownload(prato.IdFoto)
//    };

//    return CreatedAtAction(nameof(GetCardapio), new { id = prato.Id }, response);
//}


//api/cardapio/prato/{id}
//[HttpGet("prato/{id}")]
//[ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
//public IActionResult ObterPratoPeloId(int id)
//{
//    return Content($"Detalhes do prato de id {id}");
//}


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