using Restaurante.Api.DTOs;

namespace Restaurante.Api.Services
{
    /// <summary>
    /// Serviço responsável por gerenciar operações com arquivos.
    /// Realiza upload, download, listagem e remoção de arquivos da pasta "ArquivosUpload".
    /// Valida extensões e tamanhos antes de aceitar os arquivos.
    /// </summary>
    public class ArquivoService
    {
        // Caminho da pasta onde os arquivos serão armazenados no servidor
        private readonly string _pastaBase;

        // Ferramenta para registrar (log) eventos e erros durante operações com arquivos
        // Útil para debugar problemas e acompanhar o que aconteceu
        private readonly ILogger<ArquivoService> _logger;

        // Lista de extensões de arquivo que são permitidas para fazer upload
        // Apenas PDF, JPG, JPEG e PNG podem ser salvos
        private readonly string[] _extensoesPermitidas = { ".pdf", ".jpg", ".jpeg", ".png" };

        // Tamanho máximo permitido para um arquivo (5 MB)
        // 1 MB = 1024 KB = 1024 * 1024 bytes
        // Arquivos maiores que isso não poderão ser salvos
        private const long TamanhoMaximoBytes = 5 * 1024 * 1024;

        /// <summary>
        /// Construtor da classe - é executado quando uma nova instância de ArquivoService é criada.
        /// Prepara a pasta "ArquivosUpload" para armazenar os arquivos.
        /// Se a pasta não existir, ela será criada automaticamente.
        /// </summary>
        /// <param name="logger">Ferramenta para registrar eventos e erros</param>
        /// <param name="env">Informações sobre o ambiente de execução da aplicação</param>
        public ArquivoService(ILogger<ArquivoService> logger, IWebHostEnvironment env)
        {
            // Armazena a ferramenta de logging para usar em toda a classe
            _logger = logger;

            // Cria o caminho completo da pasta onde os arquivos serão salvos
            // Path.Combine junta os caminhos de forma segura para cada sistema operacional
            _pastaBase = Path.Combine(env.ContentRootPath, "ArquivosUpload");

            // Verifica se a pasta já existe no servidor
            if (!Directory.Exists(_pastaBase))
            {
                // Se não existir, cria a pasta
                Directory.CreateDirectory(_pastaBase);
                // Registra um evento informando que a pasta foi criada com sucesso
                _logger.LogInformation("Pasta de armazenamento criada em: {Caminho}", _pastaBase);
            }
        }

        /// <summary>
        /// Método privado que verifica se um arquivo pode ser salvo.
        /// Verifica se:
        ///   - O arquivo foi realmente selecionado e não está vazio
        ///   - A extensão (tipo) do arquivo é permitida
        ///   - O tamanho do arquivo não ultrapassa o limite
        /// </summary>
        /// <param name="arquivo">O arquivo enviado pelo usuário</param>
        /// <param name="mensagemErro">Mensagem explicando por que o arquivo foi rejeitado (se for o caso)</param>
        /// <returns>True se o arquivo está válido, False se houver algum problema</returns>
        private bool ValidarArquivo(IFormFile arquivo, out string mensagemErro)
        {
            // Começa sem mensagem de erro
            mensagemErro = string.Empty;

            // VALIDAÇÃO 1: Verifica se o arquivo foi selecionado e tem conteúdo
            if (arquivo == null || arquivo.Length == 0)
            {
                // Se o arquivo está vazio, define a mensagem de erro
                mensagemErro = "Arquivo nao foi selecionado ou esta vazio.";
                // Registra um aviso no log
                _logger.LogWarning("Tentativa de upload com arquivo nulo ou vazio.");
                // Retorna falso indicando que validação falhou
                return false;
            }

            // VALIDAÇÃO 2: Verifica se a extensão do arquivo é permitida
            // Path.GetExtension pega a extensão (ex: .pdf, .jpg)
            // ToLowerInvariant converte para minúsculas para padronizar
            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (!_extensoesPermitidas.Contains(extensao))
            {
                // Se a extensão não está na lista de permitidas, gera mensagem de erro
                mensagemErro = $"Extensao '{extensao}' nao permitida. Extensoes validas: {string.Join(", ", _extensoesPermitidas)}";
                _logger.LogWarning("Tentativa de upload com extensao nao permitida: {Extensao}", extensao);
                return false;
            }

            // VALIDAÇÃO 3: Verifica se o arquivo não é muito grande
            if (arquivo.Length > TamanhoMaximoBytes)
            {
                // Converte o tamanho máximo de bytes para megabytes para exibir ao usuário
                var tamanhoMB = TamanhoMaximoBytes / (1024 * 1024);
                mensagemErro = $"Arquivo excede tamanho maximo de {tamanhoMB} MB.";
                _logger.LogWarning("Tentativa de upload com arquivo acima do tamanho maximo. Tamanho: {Tamanho} bytes", arquivo.Length);
                return false;
            }

            // Se passou em todas as validações, retorna verdadeiro
            return true;
        }

        /// <summary>
        /// Realiza o upload de um arquivo para o servidor.
        /// 
        /// Fluxo:
        ///   1. Valida o arquivo (extensão, tamanho, se existe)
        ///   2. Se válido, gera um nome único para o arquivo para evitar conflitos
        ///   3. Salva o arquivo na pasta "ArquivosUpload"
        ///   4. Retorna informações sobre o arquivo salvo
        /// 
        /// "Async" no nome significa que o método pode executar sem bloquear a aplicação.
        /// "await" pausa a execução até que a cópia do arquivo termine.
        /// </summary>
        /// <param name="arquivo">Arquivo enviado pelo usuário para ser salvo</param>
        /// <returns>Uma tupla (sucesso, resultado):
        ///   - sucesso: True se tudo correu bem, False se houve problema
        ///   - resultado: Objeto com informações do arquivo salvo (ou mensagem de erro)
        /// </returns>
        public async Task<(bool sucesso, ArquivoResponseDTO resultado)> UploadArquivoAsync(IFormFile arquivo)
        {
            // Primeiro, valida se o arquivo pode ser salvo
            if (!ValidarArquivo(arquivo, out var mensagemErro))
            {
                // Se não passou na validação, retorna falso e a mensagem de erro
                return (false, new ArquivoResponseDTO
                {
                    Mensagem = mensagemErro,
                    NomeOriginal = arquivo?.FileName ?? string.Empty
                });
            }

            try
            {
                // Extrai a extensão do arquivo (ex: .pdf, .jpg)
                var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();

                // Cria um nome único para o arquivo usando GUID (Global Unique Identifier)
                // Exemplo: "a1b2c3d4-e5f6-47g8-h9i0-j1k2l3m4n5o6.pdf"
                // Isso evita que dois arquivos com o mesmo nome se sobrescrevam
                var nomeSeguro = Guid.NewGuid().ToString() + extensao;

                // Combina o caminho da pasta com o nome do arquivo para obter o caminho completo
                var caminhoCompleto = Path.Combine(_pastaBase, nomeSeguro);

                // Cria um fluxo (stream) para escrever o arquivo no disco
                // "using" garante que o arquivo seja fechado automaticamente ao terminar
                await using (var stream = new FileStream(caminhoCompleto, FileMode.Create, FileAccess.Write))
                {
                    // Copia o conteúdo do arquivo enviado para o arquivo no servidor
                    await arquivo.CopyToAsync(stream);
                }

                // Registra o sucesso do upload no log
                _logger.LogInformation(
                    "Upload realizado com sucesso. Original: {NomeOriginal}, Armazenado: {NomeArmazenado}, Tamanho: {Tamanho} bytes",
                    arquivo.FileName,
                    nomeSeguro,
                    arquivo.Length);

                // Retorna verdadeiro e um objeto com as informações do arquivo salvo
                return (true, new ArquivoResponseDTO
                {
                    NomeArmazenado = nomeSeguro,  // Nome único gerado pelo servidor
                    NomeOriginal = arquivo.FileName,  // Nome original enviado pelo usuário
                    TamanhoBytes = arquivo.Length,  // Tamanho em bytes
                    DataUpload = DateTime.UtcNow,  // Data e hora atual
                    UrlDownload = $"/api/arquivos/download/{nomeSeguro}",  // Link para baixar
                    Mensagem = "Upload realizado com sucesso."
                });
            }
            catch (Exception ex)
            {
                // Se algo der errado (disco cheio, permissões, etc.), registra o erro no log
                _logger.LogError(ex, "Erro ao realizar upload do arquivo: {NomeOriginal}", arquivo.FileName);
                // Retorna falso e a mensagem de erro
                return (false, new ArquivoResponseDTO
                {
                    Mensagem = $"Erro interno ao salvar arquivo: {ex.Message}",
                    NomeOriginal = arquivo.FileName
                });
            }
        }

        /// <summary>
        /// Lista todos os arquivos que estão armazenados na pasta "ArquivosUpload".
        /// Retorna informações sobre cada arquivo (nome, tamanho, data, etc).
        /// </summary>
        /// <returns>Uma lista com as informações de todos os arquivos. Se a pasta não existir ou estiver vazia, retorna uma lista vazia</returns>
        public List<ArquivoResponseDTO> ListarArquivos()
        {
            // Cria uma lista vazia para armazenar os dados dos arquivos
            var resultado = new List<ArquivoResponseDTO>();

            try
            {
                // Verifica se a pasta de armazenamento existe
                if (!Directory.Exists(_pastaBase))
                {
                    // Se não existir, registra uma mensagem informativa e retorna a lista vazia
                    _logger.LogInformation("Pasta de armazenamento nao existe: {Caminho}", _pastaBase);
                    return resultado;
                }

                // Obtém uma lista com todos os arquivos da pasta
                var arquivos = Directory.GetFiles(_pastaBase);

                // Percorre cada arquivo encontrado
                foreach (var caminhoArquivo in arquivos)
                {
                    // Extrai apenas o nome do arquivo do caminho completo
                    var nomeArquivo = Path.GetFileName(caminhoArquivo);

                    // Obtém informações sobre o arquivo (tamanho, data de criação, etc)
                    var info = new FileInfo(caminhoArquivo);

                    // Adiciona um objeto com as informações do arquivo à lista
                    resultado.Add(new ArquivoResponseDTO
                    {
                        NomeArmazenado = nomeArquivo,  // Nome do arquivo no servidor
                        TamanhoBytes = info.Length,  // Tamanho em bytes
                        DataUpload = info.CreationTimeUtc,  // Data que foi criado
                        UrlDownload = $"/api/arquivos/download/{nomeArquivo}",  // Link para baixar
                        Mensagem = "Arquivo disponivel para download."
                    });
                }

                // Registra no log a quantidade de arquivos listados
                _logger.LogInformation("Listagem de arquivos realizada. Total: {Total}", resultado.Count);
            }
            catch (Exception ex)
            {
                // Se algo der errado, registra o erro no log
                _logger.LogError(ex, "Erro ao listar arquivos.");
            }

            // Retorna a lista de arquivos (pode estar vazia se houve erro)
            return resultado;
        }

        /// <summary>
        /// Procura um arquivo no servidor para que o usuário possa baixá-lo.
        /// 
        /// Retorna um "FileStream" que é basicamente um fluxo de dados do arquivo.
        /// O controlador (controller) usa esse fluxo para enviar o arquivo ao navegador do usuário.
        /// </summary>
        /// <param name="nomeArquivo">Nome do arquivo a ser procurado (o nome único gerado pelo servidor)</param>
        /// <returns>Um fluxo de dados do arquivo se encontrado, ou null (nulo) se não existir</returns>
        public FileStream? ObterArquivoParaDownload(string nomeArquivo)
        {
            try
            {
                // Extrai apenas o nome do arquivo do caminho (segurança contra manipulação de caminhos)
                // Por exemplo, impede alguém de tentar acessar "../../etc/passwd"
                var nomeSeguro = Path.GetFileName(nomeArquivo);

                // Monta o caminho completo do arquivo
                var caminhoCompleto = Path.Combine(_pastaBase, nomeSeguro);

                // Verifica se o arquivo existe no servidor
                if (!File.Exists(caminhoCompleto))
                {
                    // Se não existir, registra um aviso e retorna nulo
                    _logger.LogWarning("Tentativa de download de arquivo nao encontrado: {Nome}", nomeSeguro);
                    return null;
                }

                // Registra no log que o download foi iniciado
                _logger.LogInformation("Download iniciado para arquivo: {Nome}", nomeSeguro);

                // Abre o arquivo e retorna um fluxo para lê-lo
                // FileShare.Read permite que outros usuários também leiam o arquivo ao mesmo tempo
                return new FileStream(caminhoCompleto, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception ex)
            {
                // Se algo der errado, registra o erro no log
                _logger.LogError(ex, "Erro ao obter arquivo para download: {Nome}", nomeArquivo);
                return null;
            }
        }

        /// <summary>
        /// Remove (deleta) um arquivo do servidor.
        /// A operação é permanente - o arquivo não pode ser recuperado após a deleção.
        /// </summary>
        /// <param name="nomeArquivo">Nome do arquivo a ser removido</param>
        /// <returns>True se o arquivo foi deletado com sucesso, False se não encontrou o arquivo ou houve erro</returns>
        
        public bool RemoverArquivo(string nomeArquivo)
        {
            try
            {
                // Extrai apenas o nome do arquivo do caminho (segurança contra manipulação de caminhos)
                var nomeSeguro = Path.GetFileName(nomeArquivo);

                // Monta o caminho completo do arquivo
                var caminhoCompleto = Path.Combine(_pastaBase, nomeSeguro);

                // Verifica se o arquivo existe antes de tentar deletar
                if (!File.Exists(caminhoCompleto))
                {
                    // Se não existir, registra um aviso e retorna falso
                    _logger.LogWarning("Tentativa de remover arquivo nao encontrado: {Nome}", nomeSeguro);
                    return false;
                }

                // Deleta o arquivo do disco
                File.Delete(caminhoCompleto);

                // Registra no log que a deleção foi bem-sucedida
                _logger.LogInformation("Arquivo removido com sucesso: {Nome}", nomeSeguro);

                // Retorna verdadeiro indicando que a operação funcionou
                return true;
            }
            catch (Exception ex)
            {
                // Se algo der errado (arquivo em uso, permissões insuficientes, etc), registra o erro
                _logger.LogError(ex, "Erro ao remover arquivo: {Nome}", nomeArquivo);
                return false;
            }
        }

        //Retorna caminho do arquivo
        public String CaminhoArquivo(String nomeArquivo)
        {
            var caminhoCompleto = Path.Combine(_pastaBase, nomeArquivo);

            // Verifica se o arquivo existe no servidor
            if (!File.Exists(caminhoCompleto))
            {
                // Se não existir retorna nulo
                return null;
            }
            return caminhoCompleto;
        }

        public bool ArquivoExiste(String nomeArquivo)
        {
            var caminhoCompleto = Path.Combine(_pastaBase, nomeArquivo);

            // Verifica se o arquivo existe no servidor
            if (!File.Exists(caminhoCompleto))
            {
                // Se não existir retorna nulo
                return false;
            }
            return true;
        }
    }
}
