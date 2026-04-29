using System.ComponentModel.DataAnnotations;

namespace Restaurante.Api.DTOs
{
    public class CriarPratoDTO
    {
        [Required(ErrorMessage = "Insira o nome do prato.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 150 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Insira o preço do prato.")]
        [Range(0.01, 10000.00, ErrorMessage = "O preço deve ser entre 0.01 e 10.000,00.")]
        public decimal Preco { get; set; }

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string? Descricao { get; set; }

        [StringLength(50, ErrorMessage = "A categoria não pode exceder 50 caracteres.")]
        public string? Categoria { get; set; }

        [Required(ErrorMessage = "Insira o ID da foto(nome ou url).")]
        public string IdFoto { get; set; }
    }
}





