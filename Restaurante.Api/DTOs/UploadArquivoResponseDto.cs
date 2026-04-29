namespace Restaurante.Api.DTOs;

public record UploadArquivoResponseDto(
    string nomeArmazenado,
    string urlDownload,
    string mensagem
);