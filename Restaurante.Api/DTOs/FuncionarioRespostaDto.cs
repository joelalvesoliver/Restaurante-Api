// Importa o enum "Funcao" definido no projeto SimuladorBancoDados.
using SimuladorBancoDados;

namespace Restaurante.Api.DTOs
{
    // DTO de SAÍDA (resposta): representa os dados que a API RETORNA ao cliente
    // após criar ou buscar um funcionário.
    // Perceba que este DTO NÃO tem o campo "Senha" — isso é intencional!
    // Nunca devemos retornar senhas ao cliente, mesmo que estejam criptografadas.
    // O DTO de resposta serve para expor apenas o que é necessário e seguro.
    public class FuncionarioRespostaDto
    {
        // Identificador único do funcionário gerado pelo sistema (banco de dados).
        // Todo registro tem um Id para que possamos encontrá-lo facilmente.
        public int Id { get; set; }

        // Nome completo do funcionário.
        public string Nome { get; set; }

        // E-mail do funcionário, usado também para login.
        public string Email { get; set; }

        // Cargo do funcionário (Gerente, Garcom, Atendente, etc.).
        // É um enum: um tipo especial que lista valores fixos com nomes descritivos.
        public Funcao Funcao { get; set; }

        // Indica se o funcionário está ativo (true) ou foi desativado (false) no sistema.
        // Desativar é melhor que deletar: preserva o histórico de pedidos e registros.
        public bool Ativo { get; set; }

        // Data e hora em que o funcionário foi cadastrado no sistema.
        public DateTime DataCadastro { get; set; }
    }
}
