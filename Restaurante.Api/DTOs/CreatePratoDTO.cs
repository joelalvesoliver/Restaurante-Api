using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurante.Api.DTOs
{
    public class CreatePratoDTO
    {
            [Required(ErrorMessage = "Nome é obrigatório")]
            [StringLength(150, ErrorMessage = "Nome não pode exceder 150 caracteres")]
            public string Nome { get; set; }
            [Required(ErrorMessage = "Preço é obrigatório")]
            [Column(TypeName = "decimal(10, 2)")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
            public decimal Preco { get; set; }
            [StringLength(500, ErrorMessage = "Descrição não pode exceder 500 caracteres")]
            public string Descricao { get; set; }
            [StringLength(50, ErrorMessage = "Categoria não pode exceder 50 caracteres")]
            public string Categoria { get; set; }
            [Required(ErrorMessage = "Id da foto é obrigatorio")]
            public string IdFoto { get; set; }
    }
}
