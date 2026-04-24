using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.DTOs;
using Restaurante.Api.Services;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArquivosController : ControllerBase
    {
        private readonly ArquivoService _arquivoService;
        private readonly ILogger<ArquivosController> _logger;
        public ArquivosController(ArquivoService arquivoService, ILogger<ArquivosController> logger)
        {
            _arquivoService = arquivoService;
            _logger = logger;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ArquivoResponseDTO>> Upload([FromForm(Name = "arquivo")] IFormFile arquivo)
        {
            _logger.LogInformation("Recebida requisicao de upload de arquivo: {NomeOriginal}", arquivo?.FileName);

            if(arquivo == null)
            {
                return BadRequest(new {Mensagem = "Nenhum arquivo foi fornecido" });
            }

            var (sucesso, resultado) = await _arquivoService.UploadArquivoAsync(arquivo);

            if (!sucesso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        [HttpGet("download/{nomeArquivo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Download(string nomeArquivo)
        {
            var stream = _arquivoService.ObterArquivoParaDownload(nomeArquivo);
            if(stream == null)
            {
                return NotFound(new { Mensagem = "Arquivo não encontrado" });
            }

            var contetType = GetMimeType(nomeArquivo);
            return File(stream, contetType, nomeArquivo);
        }

        [HttpGet("listar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<List<ArquivoResponseDTO>> Listar()
        {
            _logger.LogInformation("Requisicao para listar arquivos.");
            var arquivos = _arquivoService.ListarArquivos();

            if(arquivos.Count == 0)
            {
                return NoContent();
            }

            return Ok(arquivos);
        }

        [HttpDelete("remover/{nomeArquivo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Remover(string nomeArquivo)
        {
            _logger.LogInformation("Requisicao para remover arquivo: {Nome}", nomeArquivo);

            var removido = _arquivoService.RemoverArquivo(nomeArquivo);
            if (!removido)
            {
                return NotFound(new {Mensagem = "Arquivo não encontrado para remoção"});
            }

            return Ok(new { Mensagem = "Arquivo removido com sucesso" });
        }

        private static string GetMimeType(string nomeArquivo)
        {
            var extensao = Path.GetExtension(nomeArquivo).ToLowerInvariant();
            return extensao switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".txt" => "text/plain",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}


// Duvidas do Projeto e dos conceitos da Aula