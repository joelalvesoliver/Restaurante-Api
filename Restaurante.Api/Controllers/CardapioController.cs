using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurante.Api.DTOs;
using Restaurante.Api.Filtros;
using Restaurante.Api.Services;
using SimuladorBancoDados.Entidade;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Service;
using Restaurante.Api.Interface;

namespace Restaurante.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
    public class CardapioController : ControllerBase
    {
        private readonly IPratoRepository _repository;
        private readonly ILogger<CardapioController> _logger;
        private readonly IArquivoService _arquivoService;
        public CardapioController(IPratoRepository repository, ILogger<CardapioController> logger, IArquivoService arquivoService)
        {
            _repository = repository;
            _logger = logger;
            _arquivoService = arquivoService;
        }



        // Aqui está sendo implementado o requisito 1

        //api/cardapío
        [HttpGet("")]
        [ServiceFilter(typeof(LogAuditoria))]
        [ProducesResponseType(typeof(List<CardapioExibicaoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult Index([FromHeader(Name = "X-Canal")] string canal) // estamos usando o X-Canal com canal app
        {
            // 1. Logger para registrar a requisição (Exigência do Requisito 1)
            _logger.LogInformation("Canal '{0}' solicitou a listagem do cardápio.", canal);

            // 2. Busca todos os pratos usando o método solicitado
            var pratos = _repository.BuscaTodosPratos();

            // 3. RN: Se a lista estiver vazia, retornar 204 No Content
            if (pratos == null || !pratos.Any())
            {
                _logger.LogWarning("O cardápio foi solicitado mas a lista está vazia.");
                return NoContent();
            }
            //string rotaArquivo;

            // 4. Mapeamento da Entidade Dominio -> DTO de Resposta
            var listaDto = pratos.Select(p => new CardapioExibicaoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Preco = p.Preco,
                Descricao = p.Descricao ?? "Prato sem descrição disponível.",
                Categoria = p.Categoria,
                DataCadastro = p.DataCadastro,
                Foto = new ArquivoResponseDTO
                {
                    NomeOriginal = p.IdFoto,
                    NomeArmazenado = p.IdFoto,
                    // Gera a URL dinâmica para o download da foto]
                    //exibirá endpoint, separando os originais dos colocados pelo POST, os 25 primeiros são originais
                    UrlDownload = $"{Request.Scheme}://{Request.Host}/api/Arquivo/{(p.Id <= 25 ? "originais" : "download")}/{p.IdFoto}",
                    Mensagem = "Link para download gerado com sucesso."
                }
            }).ToList();

            return Ok(listaDto);
        }

        //api/cardapio código original
        /*  [HttpGet("")]
          [ServiceFilter(typeof(LogAuditoria))]
          public IActionResult Index()
          {
              return Content("Lista de pratos do cardapio");
          }*/

        //api/cardapio/prato/{id}





        // Aqui est[a sendo implementado o requisito 2

        [HttpGet("prato/{id:int:min(1)}")]
        [ServiceFilter(typeof(VerificarAutorizacaoFazerPedido))]
        /*public IActionResult ObterPratoPeloId(int id)
        {
            return Content($"Detalhes do prato de id {id}");
        } código original*/
        public IActionResult ObterPratoPeloId(int id, [FromHeader(Name = "X-Canal")] string canal)
        {
            _logger.LogInformation("Canal '{0}' solicitou detalhes do prato ID {1}.", canal, id);// Registro de aviso no log


            var prato = _repository.BuscaPratoPeloId(id); //usa rerpositorio

            if (prato == null)
            {

                return NotFound(new { mensagem = $"Prato com ID {id} não encontrado no cardápio." });
                _logger.LogWarning("Prato com ID {0} não encontrado para o canal {1}.", id, canal);
                return NotFound(new { mensagem = $"Prato com ID {id} não encontrado no cardápio." });
            }

            string rotaArquivo = (prato.Id <= 25) ? "originais" : "download"; //exibirá endpoint, separando os originais dos colocados pelo POST, os 25 primeiros são originais

            var respostaDto = new CardapioExibicaoDTO
            {
                Id = prato.Id,
                Nome = prato.Nome,
                Preco = prato.Preco,
                Descricao = prato.Descricao,
                Categoria = prato.Categoria,
                Ativo = prato.Ativo,
                DataCadastro = prato.DataCadastro,
                Foto = new ArquivoResponseDTO
                {
                    NomeOriginal = prato.IdFoto, // ID da foto exigido
                    NomeArmazenado = prato.IdFoto,
                    // Endpoint de download exigido
                    //UrlDownload = $"{Request.Scheme}://{Request.Host}/api/Arquivo/download/{prato.IdFoto}",
                    UrlDownload = $"{Request.Scheme}://{Request.Host}/api/Arquivo/{rotaArquivo}/{prato.IdFoto}",
                    Mensagem = "Informações da foto recuperadas com sucesso."
                }
            };

            return Ok(respostaDto);
        }

        // Aqui será implementado o requisito 3

        [HttpPost]
        [ProducesResponseType(typeof(CardapioExibicaoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Para o X-Canal
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Tirando o 403, todos são exigidos pelo enunciado
        public IActionResult CadastrarPrato([FromBody] CriarPratoDTO novoDto, [FromHeader(Name = "X-Canal")] string canal)
        {




            try
            {

                //Faz com que foto exista (Regra de Negócio contida na dica, na descrição)
                if (!_arquivoService.ArquivoExiste(novoDto.IdFoto))
                {
                    return BadRequest(new
                    {
                        Mensagem = "O ID da foto informado não corresponde a nenhum arquivo enviado. Realize o upload da imagem primeiro.",
                        Campo = "IdFoto"
                    });
                }

                //Bomba pra testar o tratamento de exceção e geração do status 500
                //throw new Exception("Simulação de falha crítica no servidor!");
                // DTO de POST dos dados de entrada -> Entidade de Dominio
                var prato = new Prato
                {
                    Nome = novoDto.Nome,
                    Preco = novoDto.Preco,
                    Descricao = novoDto.Descricao,
                    Categoria = novoDto.Categoria,
                    IdFoto = novoDto.IdFoto
                    // O Repositório cuidará do Id (que será 26+), Ativo e DataCadastro
                };
                // Repositorio
                _repository.AdicionaPrato(prato);

                // Logger
                _logger.LogInformation("Prato '{0}' cadastrado com ID {1}.", prato.Nome, prato.Id);

                return CreatedAtAction(nameof(ObterPratoPeloId), new { id = prato.Id, canal = canal }, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao cadastrar o prato {0}", novoDto.Nome);

                // Retornando o status 500 explicitamente
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { mensagem = "Erro interno ao processar sua solicitação." });
            }





        }



        // aqui será implementado o requisito 4



        [HttpPost("upload-foto")]
        [ProducesResponseType(typeof(ArquivoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFoto(IFormFile arquivo, [FromHeader(Name = "X-Canal")] string canal)
        {
            // logger
            _logger.LogInformation("Tentativa de upload de arquivo: {0}", arquivo?.FileName);


            try
            {

                var (sucesso, resultado) = await _arquivoService.UploadArquivoAsync(arquivo);
                //validação básica do arquivo
                if (!sucesso)
                {
                    return BadRequest(new { mensagem = "Arquivo não enviado ou vazio." });
                }

                /*  var nomeArquivo = await _arquivoService.UploadArquivoAsync(arquivo);
                  var respostaDto = new ArquivoResponseDTO
                  {
                      NomeOriginal = arquivo.FileName,
                      NomeArmazenado = nomeArquivo,
                      TamanhoBytes = arquivo.Length,
                      DataUpload = DateTime.UtcNow,
                      UrlDownload = $"{Request.Scheme}://{Request.Host}/api/Arquivo/download/{nomeArquivo}",
                      Mensagem = "Upload realizado com sucesso."
                  };
                  return Ok(respostaDto);*/
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer upload do arquivo {0}", arquivo?.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao processar o upload." });
            }




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