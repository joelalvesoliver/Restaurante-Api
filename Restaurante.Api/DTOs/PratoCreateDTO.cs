using System.ComponentModel.DataAnnotations;

namespace Restaurante.Api.DTOs;

public record PratoCreateDto
(
    [Required(ErrorMessage = "O nome do prato é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    string Nome,

    [Required(ErrorMessage = "O preço é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser superior a zero.")]
    decimal Preco,

    [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
    string? Descricao,

    [StringLength(50, ErrorMessage = "A categoria não pode exceder 50 caracteres.")]
    string? Categoria,

    [Required(ErrorMessage = "O IdFoto é obrigatório para vincular uma imagem ao prato.")]
    string IdFoto
);