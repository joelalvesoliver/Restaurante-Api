using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Restaurante.Api.DTOs
{
    public class PratoCreateDTO
    {
        [Required(ErrorMessage = "O nome do prato é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string Descricao { get; set; }
                
        [StringLength(50, ErrorMessage = "Categoria não pode exceder 50 caracteres.")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "O Id da foto é obrigatório.")]
        public string IdFoto { get; set; }

        // Define se o prato deve aparecer no cardápio
        public bool Ativo { get; set; } = true;
    }

}
