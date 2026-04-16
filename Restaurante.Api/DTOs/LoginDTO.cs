using System.ComponentModel.DataAnnotations;

namespace Restaurante.Api.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email e obrigatorio")]
        [EmailAddress(ErrorMessage = "Email em formato invalido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha e obrigatoria")]
        public string Senha  { get; set; }
    }
}
