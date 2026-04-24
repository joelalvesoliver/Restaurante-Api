// Este arquivo define duas coisas:
// 1. A classe UsuarioDto: usada para trafegar dados de usuário entre as camadas do sistema.
// 2. O enum Funcao: lista fixa das funções/cargos possíveis no restaurante.

namespace SimuladorBancoDados
{
    // DTO (Data Transfer Object) de Usuário.
    // Esta classe é usada internamente para mover os dados de um usuário
    // entre o banco de dados (simulado) e as outras camadas do sistema.
    // Diferente da entidade Usuario (que é interna ao banco), este DTO
    // pode ser criado e lido livremente por outras partes do sistema.
    public class UsuarioDto
    {
        // Identificador único do usuário (gerado automaticamente pelo sistema).
        public int Id { get; set; }

        // Nome completo do usuário.
        public string Nome { get; set; }

        // E-mail usado para login e identificação do usuário.
        public string Email { get; set; }

        // Senha do usuário. Em um sistema real, jamais armazenaríamos a senha
        // em texto puro — usaríamos um algoritmo de hash (como BCrypt).
        // Aqui, por ser um simulador educacional, a senha fica em texto simples.
        public string Senha { get; set; }

        // Função/cargo do usuário no restaurante.
        // O tipo é o enum Funcao definido logo abaixo.
        public Funcao Funcao { get; set; }

        // Indica se o usuário está ativo (true) ou inativo (false) no sistema.
        public bool Ativo { get; set; }

        // Data e hora em que o usuário foi cadastrado no sistema.
        public DateTime DataCadastro { get; set; }
    }

    // Enum (Enumeração) é um tipo especial em C# que representa
    // um conjunto fixo de valores com nomes descritivos.
    // Em vez de usar números soltos (1, 2, 3...), o enum dá um NOME a cada número,
    // tornando o código muito mais legível e evitando erros de digitação.
    // Ex: usar Funcao.Gerente é bem mais claro do que usar o número 1.
    public enum Funcao
    {
        Gerente   = 1, // Responsável pela gestão geral do restaurante
        Garcom    = 2, // Atende as mesas e faz os pedidos
        Atendente = 3, // Atendimento no balcão ou caixa
        Cozinha   = 4, // Equipe da cozinha
        Admin     = 5  // Administrador do sistema (acesso total)
    }
}
