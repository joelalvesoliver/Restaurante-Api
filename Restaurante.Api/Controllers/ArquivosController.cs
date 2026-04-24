// =============================================================================
// ARQUIVO: ArquivosController.cs
// =============================================================================
// Este Controller é responsável por todas as operações relacionadas a arquivos:
//   - Receber (upload) arquivos enviados pelo cliente
//   - Disponibilizar (download) arquivos para o cliente baixar
//   - Listar todos os arquivos salvos no servidor
//   - Remover arquivos do servidor
//
// O que é um Controller?
// Em uma API REST, o Controller é a "porta de entrada" das requisições.
// Ele recebe a requisição HTTP, chama o serviço (lógica de negócio) e
// devolve a resposta ao cliente. O Controller não deve ter lógica complexa —
// essa responsabilidade fica no ArquivoService.
// =============================================================================

// "using" importa namespaces (pacotes/bibliotecas) para usar neste arquivo.
// Sem essas importações, as classes abaixo não seriam reconhecidas pelo compilador.
using Microsoft.AspNetCore.Mvc;       // Fornece ControllerBase, HttpGet, HttpPost, etc.
using Restaurante.Api.DTOs;           // Fornece o ArquivoResponseDTO
using Restaurante.Api.Services;       // Fornece o ArquivoService

namespace Restaurante.Api.Controllers
{
    // [ApiController] é um ATRIBUTO — uma "etiqueta" que configura comportamentos automáticos:
    //   1. Valida o ModelState automaticamente e retorna 400 se os dados forem inválidos
    //   2. Infere de onde vêm os parâmetros ([FromBody], [FromRoute], etc.) sem precisar declarar
    //   3. Formata erros de validação no padrão ProblemDetails
    // Sem este atributo, precisaríamos fazer essas verificações manualmente em cada método.
    [ApiController]

    // [Route] define o prefixo de URL para TODOS os endpoints desta classe.
    // "api/[controller]" → [controller] é substituído automaticamente pelo nome da classe
    // removendo o sufixo "Controller". Logo: "ArquivosController" → "api/arquivos"
    // Todos os endpoints aqui começam com: https://servidor/api/arquivos/...
    [Route("api/[controller]")]

    // "ControllerBase" é a classe pai que fornece métodos prontos como:
    // Ok(), BadRequest(), NotFound(), File(), CreatedAtRoute(), etc.
    // Esses métodos facilitam retornar as respostas HTTP corretas com o corpo adequado.
    // (Diferente de "Controller", o ControllerBase não tem suporte a Views/HTML — só API)
    public class ArquivosController : ControllerBase
    {
        // Campos privados que guardam as dependências injetadas pelo construtor.
        // "readonly" = só podem ser atribuídos no construtor — nunca alterados depois.
        // O prefixo "_" é uma convenção de nomenclatura para campos privados de classe.

        // ArquivoService contém toda a lógica de negócio (validar, salvar, listar, remover).
        // O Controller delega o trabalho pesado para o Service — isso segue o princípio de
        // Separação de Responsabilidades (Separation of Concerns):
        //   Controller → recebe a requisição e decide o que fazer
        //   Service    → executa a lógica de negócio
        //   Repository → acessa o banco de dados (não utilizado diretamente aqui)
        private readonly ArquivoService _arquivoService;

        // ILogger: ferramenta oficial do .NET para registrar eventos em log.
        // Registrar logs é fundamental para monitorar a aplicação em produção.
        // Ex: saber quantos uploads ocorreram, quais arquivos foram removidos, erros, etc.
        // O tipo genérico <ArquivosController> define a "categoria" do log —
        // facilita filtrar nos sistemas de monitoramento qual classe gerou cada mensagem.
        private readonly ILogger<ArquivosController> _logger;

        // CONSTRUTOR: método especial executado quando o .NET cria uma instância deste controller.
        // O ASP.NET Core usa INJEÇÃO DE DEPENDÊNCIA (Dependency Injection) para fornecer
        // os objetos automaticamente: em vez de criarmos "new ArquivoService()" manualmente,
        // o framework cria e injeta os objetos já configurados.
        //
        // Benefícios da injeção de dependência:
        //   - Facilita testes unitários (podemos injetar versões "falsas" dos serviços)
        //   - Permite trocar implementações sem alterar este arquivo
        //   - Centraliza a criação de objetos no Program.cs
        public ArquivosController(ArquivoService arquivoService, ILogger<ArquivosController> logger)
        {
            // Armazena as dependências nos campos privados para uso nos métodos abaixo.
            _arquivoService = arquivoService;
            _logger = logger;
        }

        // =====================================================================
        // ENDPOINT 1: UPLOAD DE ARQUIVO
        // URL: POST https://servidor/api/arquivos/upload
        // =====================================================================

        // [HttpPost("upload")]: este endpoint responde APENAS a requisições HTTP POST.
        // POST é o verbo HTTP usado quando queremos CRIAR algo no servidor.
        // A string "upload" é concatenada à rota base → URL final: api/arquivos/upload
        [HttpPost("upload")]

        // [Consumes("multipart/form-data")]: define o formato de dados que este endpoint aceita.
        // "multipart/form-data" é o formato HTTP criado especificamente para envio de arquivos.
        //
        // Por que não usar JSON para enviar arquivos?
        // JSON é texto puro — não consegue representar dados binários (imagens, PDFs)
        // de forma eficiente. O formato multipart/form-data divide a requisição em "partes",
        // permitindo enviar campos de texto e arquivos binários juntos na mesma requisição.
        [Consumes("multipart/form-data")]

        // [ProducesResponseType]: documenta os possíveis códigos de resposta HTTP deste endpoint.
        // Não altera o comportamento — serve para gerar documentação automática no Swagger
        // e comunicar a outros desenvolvedores o que esperar desta rota.
        [ProducesResponseType(StatusCodes.Status201Created)]             // Arquivo criado com sucesso
        [ProducesResponseType(StatusCodes.Status400BadRequest)]          // Requisição inválida
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erro inesperado no servidor

        // Task<ActionResult<T>>: tipo de retorno de métodos assíncronos.
        //   - "Task" indica que o método é assíncrono — não bloqueia a thread enquanto salva o arquivo,
        //     permitindo que o servidor continue atendendo outras requisições simultaneamente.
        //   - "ActionResult<ArquivoResponseDTO>" permite retornar tanto um resultado HTTP (200, 400...)
        //     quanto um objeto tipado (ArquivoResponseDTO) serializado como JSON na resposta.
        //
        // [FromForm(Name = "arquivo")]: indica que o parâmetro vem do formulário multipart,
        // especificamente no campo de nome "arquivo". O cliente deve enviar o arquivo com este nome.
        //
        // IFormFile: tipo especial do ASP.NET que representa um arquivo recebido via HTTP.
        // Dá acesso ao nome original, tamanho em bytes, tipo do conteúdo e o próprio conteúdo binário.
        public async Task<ActionResult<ArquivoResponseDTO>> Upload([FromForm(Name = "arquivo")] IFormFile arquivo)
        {
            // LogInformation: registra uma mensagem de nível "informação" no log.
            // Os níveis de log (do menos para o mais grave) são:
            //   Trace → Debug → Information → Warning → Error → Critical
            //
            // A sintaxe "{NomeOriginal}" é um placeholder — o valor real é passado como argumento.
            // Usar placeholders (em vez de concatenar strings) é mais eficiente e evita
            // processar a string quando o log está desativado naquele nível.
            //
            // O operador "?." é o "null-conditional": se "arquivo" for null, retorna null
            // em vez de lançar NullReferenceException — evitando crash em uma validação de log.
            _logger.LogInformation("Recebida requisicao de upload de arquivo: {NomeOriginal}", arquivo?.FileName);

            // Primeira validação: verifica se o cliente realmente enviou um arquivo.
            // Se o campo "arquivo" não foi preenchido no formulário, a variável será null.
            if (arquivo == null)
            {
                // BadRequest() retorna HTTP 400 — "a requisição está incorreta/incompleta".
                // É usado quando o CLIENTE cometeu um erro (dados faltando, formato errado, etc.).
                // "new { }" cria um objeto anônimo apenas para montar o JSON de resposta.
                // O cliente receberá: { "Mensagem": "Nenhum arquivo foi fornecido" }
                return BadRequest(new { Mensagem = "Nenhum arquivo foi fornecido" });
            }

            // Delega o processamento real ao ArquivoService:
            //   - Valida a extensão do arquivo (.pdf, .jpg, .png são aceitos)
            //   - Valida o tamanho (máximo 5 MB)
            //   - Gera um nome único (GUID) para evitar conflitos
            //   - Salva o arquivo na pasta "ArquivosUpload" do servidor
            //
            // "await" pausa a execução deste método até que o Service termine,
            // MAS sem bloquear a thread do servidor — ela fica livre para outras requisições.
            // Isso é programação ASSÍNCRONA: importante para operações de I/O (disco, rede).
            //
            // O resultado é uma TUPLA (dois valores retornados de uma só vez).
            // A desestruturação "var (sucesso, resultado)" extrai os dois valores diretamente:
            //   - sucesso: bool indicando se o upload foi bem-sucedido
            //   - resultado: ArquivoResponseDTO com dados do arquivo salvo ou mensagem de erro
            var (sucesso, resultado) = await _arquivoService.UploadArquivoAsync(arquivo);

            // Se o Service indicou falha (extensão inválida, arquivo muito grande, etc.),
            // retornamos 400 com o objeto "resultado" que contém a mensagem de erro detalhada.
            if (!sucesso)
            {
                return BadRequest(resultado);
            }

            // Ok() retorna HTTP 200 com o "resultado" serializado como JSON no corpo da resposta.
            // "resultado" é um ArquivoResponseDTO com: nome armazenado, tamanho, URL de download, etc.
            return Ok(resultado);
        }

        // =====================================================================
        // ENDPOINT 2: DOWNLOAD DE ARQUIVO
        // URL: GET https://servidor/api/arquivos/download/{nomeArquivo}
        // =====================================================================

        // [HttpGet("download/{nomeArquivo}")]: responde a GET com um parâmetro de rota dinâmico.
        // GET é o verbo HTTP para BUSCAR/LER dados sem modificar nada no servidor.
        // "{nomeArquivo}" é um segmento variável da URL — qualquer valor é capturado automaticamente
        // e passado para o parâmetro "nomeArquivo" do método.
        // Exemplo: GET /api/arquivos/download/a3f2c1d0-4e5b.pdf → nomeArquivo = "a3f2c1d0-4e5b.pdf"
        [HttpGet("download/{nomeArquivo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]       // Arquivo encontrado e enviado
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Arquivo não existe no servidor

        // ActionResult (sem o tipo genérico <T>): usado quando retornamos respostas diversas.
        // Sem o genérico porque o retorno principal é um arquivo binário, não um objeto JSON.
        public ActionResult Download(string nomeArquivo)
        {
            // Solicita ao Service um "stream" (fluxo) do arquivo para download.
            //
            // O que é um Stream?
            // Um Stream é uma abstração para ler/escrever dados em pedaços sequencialmente,
            // sem precisar carregar o arquivo inteiro na memória RAM de uma vez.
            // Fundamental para arquivos grandes — um PDF de 100 MB não precisa ficar
            // 100 MB na RAM, o servidor envia aos poucos enquanto o cliente recebe.
            //
            // O Service retorna null se o arquivo não existir na pasta do servidor.
            var stream = _arquivoService.ObterArquivoParaDownload(nomeArquivo);

            if (stream == null)
            {
                // 404 Not Found: o arquivo solicitado não existe no servidor.
                return NotFound(new { Mensagem = "Arquivo não encontrado" });
            }

            // Identifica o tipo MIME do arquivo com base na extensão.
            // MIME Type (Multipurpose Internet Mail Extensions) é um identificador padronizado
            // que diz ao browser/cliente COMO interpretar e abrir o arquivo recebido.
            // Exemplos:
            //   "image/jpeg"       → o browser exibe como imagem
            //   "application/pdf"  → o browser abre no leitor de PDF embutido
            //   "text/plain"       → o browser exibe como texto
            //   "application/octet-stream" → o browser força o download sem tentar abrir
            var contetType = GetMimeType(nomeArquivo);

            // File() é um método herdado de ControllerBase que retorna o arquivo como resposta HTTP.
            // O ASP.NET Core cuida de enviar o stream para o cliente de forma eficiente.
            // Parâmetros:
            //   1. stream       → o conteúdo binário do arquivo
            //   2. contetType   → o tipo MIME (para o browser saber como tratar)
            //   3. nomeArquivo  → o nome sugerido para salvar no computador do cliente
            return File(stream, contetType, nomeArquivo);
        }

        // =====================================================================
        // ENDPOINT 3: LISTAR ARQUIVOS
        // URL: GET https://servidor/api/arquivos/listar
        // =====================================================================
        // Retorna uma lista com informações de todos os arquivos salvos no servidor.
        // Útil para que o cliente saiba quais arquivos existem e possa escolher qual baixar.
        [HttpGet("listar")]
        [ProducesResponseType(StatusCodes.Status200OK)]        // Lista retornada com sucesso
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Servidor ok, mas sem arquivos

        // List<ArquivoResponseDTO>: lista de objetos DTO com informações de cada arquivo.
        public ActionResult<List<ArquivoResponseDTO>> Listar()
        {
            _logger.LogInformation("Requisicao para listar arquivos.");

            // Pede ao Service a lista de todos os arquivos salvos na pasta do servidor.
            var arquivos = _arquivoService.ListarArquivos();

            // ".Count" retorna o número de itens na lista — 0 significa lista vazia.
            if (arquivos.Count == 0)
            {
                // 204 No Content: operação bem-sucedida, mas não há dados para retornar.
                //
                // Diferença entre 204 e 404 — esta distinção é importante:
                //   404 Not Found    = o recurso não existe (URL errada, pasta não encontrada)
                //   204 No Content   = o recurso existe, mas está vazio (pasta existe, sem arquivos)
                //
                // Retornar 404 seria semanticamente errado aqui: a rota /listar existe e
                // funcionou corretamente — simplesmente não há arquivos cadastrados ainda.
                return NoContent();
            }

            // Retorna 200 com a lista de arquivos serializada como array JSON.
            return Ok(arquivos);
        }

        // =====================================================================
        // ENDPOINT 4: REMOVER ARQUIVO
        // URL: DELETE https://servidor/api/arquivos/remover/{nomeArquivo}
        // =====================================================================

        // [HttpDelete]: responde ao verbo HTTP DELETE, semanticamente correto para REMOVER recursos.
        // Por que não usar POST para deletar? Seria tecnicamente possível, mas violaria o padrão REST.
        // REST define que cada verbo HTTP tem uma semântica clara:
        //   GET    → ler/buscar dados (sem efeitos colaterais)
        //   POST   → criar um novo recurso
        //   PUT    → substituir um recurso existente por completo
        //   PATCH  → atualizar parcialmente um recurso
        //   DELETE → remover um recurso
        // Seguir estas convenções torna a API mais intuitiva e previsível para quem a consome.
        [HttpDelete("remover/{nomeArquivo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]        // Removido com sucesso
        [ProducesResponseType(StatusCodes.Status404NotFound)]  // Arquivo não encontrado
        public ActionResult Remover(string nomeArquivo)
        {
            // Registra no log qual arquivo está sendo removido — importante para auditoria.
            // Em produção, logs de remoção ajudam a rastrear se algo foi deletado por engano.
            _logger.LogInformation("Requisicao para remover arquivo: {Nome}", nomeArquivo);

            // Pede ao Service para tentar remover o arquivo.
            // O Service retorna true se removeu com sucesso, false se o arquivo não existia.
            var removido = _arquivoService.RemoverArquivo(nomeArquivo);

            if (!removido)
            {
                // 404: o arquivo não foi encontrado para ser removido.
                return NotFound(new { Mensagem = "Arquivo não encontrado para remoção" });
            }

            // 200 OK com mensagem de confirmação.
            // Poderíamos também usar 204 No Content aqui, mas optamos por 200 para
            // retornar uma mensagem informando explicitamente que a remoção ocorreu.
            return Ok(new { Mensagem = "Arquivo removido com sucesso" });
        }

        // =====================================================================
        // MÉTODO AUXILIAR PRIVADO: GetMimeType
        // =====================================================================
        // "private": só pode ser chamado dentro desta classe — não é exposto como endpoint.
        // "static": não depende de nenhum estado da instância (não acessa "_arquivoService"
        //           nem "_logger"). Métodos estáticos são ligeiramente mais eficientes.
        //
        // Responsabilidade: mapear a extensão do arquivo para seu tipo MIME correspondente.
        // Separamos em um método próprio para não poluir o método Download com essa lógica.
        // Se precisarmos suportar novas extensões, alteramos apenas aqui.
        private static string GetMimeType(string nomeArquivo)
        {
            // Path.GetExtension: extrai apenas a extensão do nome do arquivo.
            //   Ex: "relatorio-2024.PDF"  → ".PDF"
            //   Ex: "foto.jpg"            → ".jpg"
            //   Ex: "sem-extensao"        → "" (string vazia)
            //
            // ToLowerInvariant: converte para minúsculas de forma consistente.
            //   "InvariantCulture" significa independente do idioma/locale do sistema operacional.
            //   Ex: ".PDF" e ".pdf" passam a ser iguais → evita tratar como extensões diferentes.
            var extensao = Path.GetExtension(nomeArquivo).ToLowerInvariant();

            // "switch expression": forma moderna (C# 8+) e compacta de escrever múltiplos if/else.
            // Compara o valor de "extensao" com cada padrão e retorna o resultado correspondente.
            // O compilador avisa se algum caso estiver duplicado e exige o caso padrão "_".
            return extensao switch
            {
                ".pdf"  => "application/pdf",    // Documentos PDF (Adobe Acrobat)
                ".jpg" or ".jpeg" => "image/jpeg", // Imagens JPEG — "or" aceita dois valores no mesmo caso
                ".png"  => "image/png",           // Imagens PNG (suporta transparência)
                ".txt"  => "text/plain",          // Arquivos de texto simples
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // Word
                // "_" é o caso padrão — executado quando nenhum dos casos acima combina.
                // "application/octet-stream" significa "fluxo de bytes genérico":
                // instrui o browser a fazer download direto sem tentar abrir ou interpretar o arquivo.
                _ => "application/octet-stream"
            };
        }
    }
}


// Duvidas do Projeto e dos conceitos da Aula