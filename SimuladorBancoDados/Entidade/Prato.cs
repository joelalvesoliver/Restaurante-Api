// Importa ferramentas para validação e mapeamento de banco de dados.
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimuladorBancoDados.Entidade
{
    // "Entidade" representa um objeto do mundo real que tem importância para o negócio.
    // A classe Prato representa um item do cardápio do restaurante.
    // Em um projeto com banco de dados (ex: Entity Framework), esta classe
    // seria mapeada diretamente para uma tabela no banco de dados.
    public class Prato
    {
        // Identificador único do prato no banco de dados.
        // Em bancos relacionais, é a chave primária (PRIMARY KEY) da tabela.
        public int Id { get; set; }

        // Nome do prato no cardápio. Ex: "Frango Grelhado", "Sopa do Dia".
        // [Required] → obrigatório, não pode ser nulo ou vazio.
        // [StringLength(150)] → máximo de 150 caracteres.
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(150, ErrorMessage = "Nome não pode exceder 150 caracteres")]
        public string Nome { get; set; }

        // Preço do prato. Ex: 29.90 (reais).
        // [Required] → obrigatório.
        // [Column(TypeName = "decimal(10,2)")] → define como este campo será armazenado
        // no banco de dados: número decimal com até 10 dígitos, sendo 2 decimais.
        // [Range] → garante que o preço seja sempre maior que zero.
        [Required(ErrorMessage = "Preço é obrigatório")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Preco { get; set; }

        // Descrição do prato, explicando os ingredientes ou preparo.
        // Opcional (sem [Required]), mas limitado a 500 caracteres.
        [StringLength(500, ErrorMessage = "Descrição não pode exceder 500 caracteres")]
        public string Descricao { get; set; }

        // Categoria a que o prato pertence. Ex: "Massas", "Saladas", "Sobremesas".
        // Usado para agrupar os itens do cardápio.
        [StringLength(50, ErrorMessage = "Categoria não pode exceder 50 caracteres")]
        public string Categoria { get; set; }

        // Identificador da foto do prato (geralmente o nome do arquivo ou uma URL).
        // Necessário para exibir a imagem do prato no aplicativo ou site.
        [Required(ErrorMessage = "Id da foto é obrigatorio")]
        public string IdFoto { get; set; }

        // Indica se o prato está disponível (ativo = true) ou retirado do cardápio (false).
        // O valor padrão já é true: todo prato é criado como ativo automaticamente.
        // "= true" define o valor inicial da propriedade quando o objeto é criado.
        public bool Ativo { get; set; } = true;

        // Data e hora em que o prato foi cadastrado no sistema.
        // "= DateTime.Now" define a data atual como padrão automaticamente.
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}
