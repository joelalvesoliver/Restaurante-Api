// Importa o enum "Funcao" que está definido no projeto SimuladorBancoDados.
using SimuladorBancoDados;
// Importa as ferramentas de validação de dados do .NET.
// Essas ferramentas permitem colocar "regras" diretamente nas propriedades da classe.
using System.ComponentModel.DataAnnotations;

namespace Restaurante.Api.DTOs
{
    // DTO de ENTRADA: representa os dados que o cliente deve enviar
    // ao criar um novo funcionário via API.
    // DTO de entrada é diferente do de saída: aqui pedimos a senha, por exemplo,
    // mas na resposta (FuncionarioRespostaDto) nunca devolvemos a senha por segurança.
    public class FuncionarioDTO
    {
        // [Required] é uma "Data Annotation" (anotação de dados).
        // Ela instrui o .NET a validar automaticamente se este campo foi preenchido.
        // Se o cliente enviar o JSON sem o campo "Nome", a API retornará
        // automaticamente um erro 400 com a mensagem definida.
        [Required(ErrorMessage = "Nome e obrigatorio")]
        // [StringLength] define os limites de tamanho do texto.
        // Aqui: o nome deve ter no mínimo 3 e no máximo 150 caracteres.
        [StringLength(150, MinimumLength = 3,
        ErrorMessage = "Nome deve ter entre 3 e 150 caracteres")]
        public string Nome { get; set; }

        // Campo obrigatório para o e-mail do funcionário.
        [Required(ErrorMessage = "Email e obrigatorio")]
        // [EmailAddress] valida se o texto está no formato de e-mail válido.
        // Ex: "maria@restaurante.com" é válido, "maria" não é.
        [EmailAddress(ErrorMessage = "Email em formato invalido")]
        public string Email { get; set; }

        // Campo obrigatório para a senha do funcionário.
        [Required(ErrorMessage = "Senha e obrigatoria")]
        // A senha deve ter entre 6 e 20 caracteres.
        [StringLength(20, MinimumLength = 6,
            ErrorMessage = "Senha deve ter no minimo 6 caracteres")]
        public string Senha { get; set; }

        // Função/cargo do funcionário no restaurante.
        // "Funcao" é um enum, ou seja, um conjunto de valores fixos e nomeados.
        // Ex: Gerente = 1, Garcom = 2, etc. Isso evita erros de digitação.
        public Funcao Funcao { get; set; }
    }
}
