using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.DTOs;
using Restaurante.Api.Services;
using SimuladorBancoDados.Entidade;
using SimuladorBancoDados.Interfaces;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    //[ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
    public class CardapioController : ControllerBase
    {
        private readonly IPratoRepository _pratoRepository;
        private readonly ILogger<CardapioController> _logger;
        private readonly ArquivoService _arquivoService;

        public CardapioController(ILogger<CardapioController> logger,
            IPratoRepository pratoRepository,
            ArquivoService arquivoService)
        {
            _logger = logger;
            _pratoRepository = pratoRepository;
            _arquivoService = arquivoService;
        }

        [HttpPost("upload-foto")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArquivoResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFoto(IFormFile arquivo)
        {
            _logger.LogInformation("Tentativa de upload de foto: {NomeArquivo}", arquivo.FileName);

            if (arquivo.Length == 0)
            {
                _logger.LogWarning("Upload rejeitado: arquivo nulo ou vazio.");
                return BadRequest(new { mensagem = "Arquivo inválido ou não fornecido." });
            }

            try
            {
                var uploadArquivoResponse = await _arquivoService.UploadArquivoAsync(arquivo);

                _logger.LogInformation("Upload concluído com sucesso. ID Gerado: {IdFoto}",
                    uploadArquivoResponse.resultado.NomeArmazenado);

                var response = new UploadArquivoResponseDto
                (uploadArquivoResponse.resultado.NomeArmazenado, uploadArquivoResponse.resultado.UrlDownload,
                    uploadArquivoResponse.resultado.Mensagem
                );
                

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação no upload.");
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de I/O durante o upload do arquivo.");
                return StatusCode(500, "Erro interno ao processar o armazenamento da imagem.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Prato))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CadastrarPrato([FromBody] PratoCreateDto dto)
        {
            _logger.LogInformation("Iniciando cadastro de prato: {Nome}", dto.Nome);

            try
            {
                var fotoExiste = _arquivoService.ArquivoExiste(dto.IdFoto);

                if (!fotoExiste)
                {
                    _logger.LogWarning("Falha ao cadastrar prato: Foto ID {IdFoto} não encontrada.", dto.IdFoto);
                    return BadRequest(new
                        { mensagem = "O IdFoto fornecido não corresponde a um arquivo válido no servidor." });
                }

                var novoPrato = new Prato
                {
                    Nome = dto.Nome,
                    Preco = dto.Preco,
                    Descricao = dto.Descricao,
                    Categoria = dto.Categoria,
                    IdFoto = dto.IdFoto,
                    Ativo = true,
                    DataCadastro = DateTime.Now
                };

                _pratoRepository.AdicionaPrato(novoPrato);

                _logger.LogInformation("Prato {Nome} cadastrado com sucesso. ID: {Id}", novoPrato.Nome, novoPrato.Id);

                // Retorna 201 e o objeto criado (incluindo o ID gerado pelo repositório)
                return CreatedAtAction(nameof(ObterPratoPeloId), new { id = novoPrato.Id }, novoPrato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não previsto ao cadastrar o prato {Nome}", dto.Nome);
                return StatusCode(500, "Ocorreu um erro interno ao salvar o prato.");
            }
        }

        [HttpGet("prato/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PratoResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ObterPratoPeloId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Busca abortada: ID inválido fornecido ({Id}).", id);
                return BadRequest(new { mensagem = "O ID do prato deve ser um número inteiro positivo." });
            }

            _logger.LogInformation("Processando busca pelo prato ID: {Id}", id);

            try
            {
                var prato = _pratoRepository.BuscaPratoPeloId(id);

                if (prato == null)
                {
                    _logger.LogWarning("Prato com ID {Id} não encontrado.", id);
                    return NotFound(new { mensagem = $"Não foi possível encontrar um prato com o ID {id}." });
                }

                var urlDownload = $"/api/arquivos/download/{prato.IdFoto}";

                var response = new PratoResponseDto(
                    prato.Id,
                    prato.Nome,
                    prato.Preco,
                    prato.Descricao,
                    prato.Categoria,
                    prato.IdFoto,
                    urlDownload,
                    prato.Ativo,
                    prato.DataCadastro
                );

                _logger.LogInformation("Prato {Id} localizado com sucesso.", id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha crítica ao buscar prato ID {Id}.", id);
                return StatusCode(500, "Ocorreu um erro interno ao processar sua solicitação.");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PratoResponseDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ListarTodos()
        {
            _logger.LogInformation("Iniciando a busca por todos os pratos do cardápio.");

            try
            {
                var pratos = _pratoRepository.BuscaTodosPratos();

                if (!pratos.Any())
                {
                    _logger.LogInformation("Busca concluída: nenhum prato cadastrado.");
                    return NoContent();
                }

                var listaResponse = pratos.Select(p => new PratoResponseDto(
                    p.Id,
                    p.Nome,
                    p.Preco,
                    p.Descricao,
                    p.Categoria,
                    p.IdFoto,
                    $"/api/cardapio/download/{p.IdFoto}",
                    p.Ativo,
                    p.DataCadastro
                ));

                _logger.LogInformation("Listagem realizada com sucesso. Total: {Count} pratos.", pratos.Count());

                return Ok(listaResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar listar os pratos.");
                return StatusCode(500, "Erro interno ao processar a listagem do cardápio.");
            }
        }

        [HttpGet("foto/{nomeArquivo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DownloadFoto(string nomeArquivo)
        {
            _logger.LogInformation("Solicitado download da foto: {NomeArquivo}", nomeArquivo);

            if (string.IsNullOrWhiteSpace(nomeArquivo))
            {
                return BadRequest(new { mensagem = "O nome do arquivo deve ser fornecido." });
            }

            try
            {
                
                var arquivo = _arquivoService.ObterArquivoParaDownload(nomeArquivo);

                if (arquivo == null)
                {
                    _logger.LogWarning("Arquivo {NomeArquivo} não localizado no servidor.", nomeArquivo);
                    return NotFound(new { mensagem = "A imagem solicitada não foi encontrada." });
                }
                
                var contentType = ObterContentType(nomeArquivo);

                _logger.LogInformation("Download do arquivo {NomeArquivo} iniciado com sucesso.", nomeArquivo);
                
                return File(arquivo, contentType, nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar o download da foto {NomeArquivo}", nomeArquivo);
                return StatusCode(500, "Erro interno ao recuperar o arquivo.");
            }
        }

        private string ObterContentType(string nomeArquivo)
        {
            var extensao = Path.GetExtension(nomeArquivo).ToLowerInvariant();
            return extensao switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream", // Tipo genérico para binários
            };
        }
    }
}
