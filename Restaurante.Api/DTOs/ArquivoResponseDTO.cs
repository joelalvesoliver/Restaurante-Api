// DTO significa "Data Transfer Object" (Objeto de Transferência de Dados).
// A ideia do DTO é criar um objeto específico para "trafegar" dados entre partes do sistema.
// Por exemplo: o banco de dados pode ter uma tabela de arquivos com muitos campos,
// mas a API só precisa retornar algumas informações ao cliente — o DTO resolve isso.
// Aqui, este DTO representa o que a API devolve ao cliente após uma operação com arquivos.

namespace Restaurante.Api.DTOs
{
    // Classe que representa as informações retornadas ao cliente
    // após realizar operações com arquivos (upload, listagem, etc.).
    public class ArquivoResponseDTO
    {
        // Nome que o SERVIDOR deu ao arquivo para armazená-lo.
        // O servidor renomeia o arquivo usando um GUID (identificador único global)
        // para evitar que dois arquivos com o mesmo nome original se sobrescrevam.
        // Ex: "a3f2c1d0-4e5b-47f8-9abc.pdf"
        public string NomeArmazenado { get; set; } = string.Empty;

        // Nome original do arquivo, como o usuário o nomeou no computador.
        // Ex: "foto-produto.jpg" ou "cardapio-2024.pdf"
        public string NomeOriginal { get; set; } = string.Empty;

        // Tamanho do arquivo medido em bytes.
        // Para converter: 1 KB = 1024 bytes | 1 MB = 1024 * 1024 bytes
        // Ex: 2097152 bytes = 2 MB
        public long TamanhoBytes { get; set; }

        // Data e hora em que o arquivo foi enviado (feito o upload) para o servidor.
        public DateTime DataUpload { get; set; }

        // Endereço (URL) que pode ser usado para baixar o arquivo.
        // Ex: "/api/arquivos/download/a3f2c1d0-4e5b-47f8-9abc.pdf"
        public string UrlDownload { get; set; } = string.Empty;

        // Mensagem de retorno informando o resultado da operação.
        // Ex: "Upload realizado com sucesso" ou "Extensão não permitida".
        public string Mensagem { get; set; } = string.Empty;
    }
}
