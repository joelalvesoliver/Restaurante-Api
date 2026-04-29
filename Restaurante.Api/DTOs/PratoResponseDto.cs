namespace Restaurante.Api.DTOs;

public record PratoResponseDto(
    int Id,
    string Nome,
    decimal Preco,
    string? Descricao,
    string? Categoria,
    string IdFoto,
    string? UrlDownload,
    bool Ativo,
    DateTime DataCadastro
);