namespace Restaurante.Api.DTOs

{
    public class ArquivoResponseDTO
    {
        public string NomeArmazenado { get; set; } = string.Empty;
        public string NomeOriginal { get; set; } = string.Empty;
        public long TamanhoBytes { get; set; }
        public DateTime DataUpload { get; set; }
        public string UrlDownload { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
    }
}
