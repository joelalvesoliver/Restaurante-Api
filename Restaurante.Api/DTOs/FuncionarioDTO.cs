using System.ComponentModel.DataAnnotations;

namespace Restaurante.Api.DTOs
{
    public class FuncionarioDTO
    {
        [Required(ErrorMessage = "Nome e obrigatorio")]
        [StringLength(150, MinimumLength = 3,
        ErrorMessage = "Nome deve ter entre 3 e 150 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Email e obrigatorio")]
        [EmailAddress(ErrorMessage = "Email em formato invalido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha e obrigatoria")]
        [StringLength(20, MinimumLength = 6,
            ErrorMessage = "Senha deve ter no minimo 6 caracteres")]
        public string Senha { get; set; }

        public int Funcao { get; set; }
    }
}
