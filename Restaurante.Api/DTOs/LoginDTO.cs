// Importa as ferramentas de validação de dados do .NET.
using System.ComponentModel.DataAnnotations;

namespace Restaurante.Api.DTOs
{
    // DTO usado na operação de LOGIN.
    // Quando um usuário quer se autenticar (fazer login), ele envia apenas
    // o e-mail e a senha — não precisa enviar o nome ou outros dados.
    // Por isso criamos um DTO específico para isso, com só os campos necessários.
    // Isso segue o princípio de "mínimo necessário" — não pedir mais dados do que o preciso.
    public class LoginDTO
    {
        // E-mail do usuário, usado como identificador único para o login.
        // [Required] garante que o campo não pode ficar vazio.
        [Required(ErrorMessage = "Email e obrigatorio")]
        // [EmailAddress] valida se o texto tem o formato correto de e-mail.
        // Ex: "admin@restaurante.com" é válido. "admin" ou "admin@" não são.
        [EmailAddress(ErrorMessage = "Email em formato invalido")]
        public string Email { get; set; }

        // Senha do usuário para autenticação.
        // [Required] garante que a senha não pode ser enviada em branco.
        [Required(ErrorMessage = "Senha e obrigatoria")]
        public string Senha  { get; set; }
    }
}
