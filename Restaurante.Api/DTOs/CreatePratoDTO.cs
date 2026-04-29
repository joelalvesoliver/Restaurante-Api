using System.ComponentModel.DataAnnotations;

namespace Restaurante.Api.DTOs
{
    public class CreatePratoDTO
    {

        [Required(ErrorMessage = "O nome do prato é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string Descricao { get; set; }

        [StringLength(50, ErrorMessage = "A categoria deve ter no máximo 50 caracteres.")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "O ID da foto é obrigatório.")]
        public string IdFoto { get; set; }

    }
}
