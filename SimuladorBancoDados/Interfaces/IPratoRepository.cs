using SimuladorBancoDados.Entidade;

namespace SimuladorBancoDados.Interfaces
{
    // INTERFACE do repositório de Pratos.
    // Um "Repository" (Repositório) é um padrão de design muito comum em APIs.
    // Ele centraliza todas as operações de acesso a dados de uma entidade específica.
    // Assim, o controlador (Controller) não precisa saber COMO os dados são salvos—
    // ele apenas chama os métodos do repositório.
    //
    // Esta interface define o "contrato" para qualquer classe que queira
    // ser um repositório de Pratos. A implementação real é feita em PratoRepository.cs.
    public interface IPratoRepository
    {
        // Busca um prato específico pelo seu Id. Retorna null se não existir.
        Prato? BuscaPratoPeloId(int id);

        // Retorna todos os pratos cadastrados, independente do status.
        List<Prato> BuscaTodosPratos();

        // Retorna apenas os pratos de uma categoria específica.
        // Ex: passar "Sobremesas" retorna só as sobremesas do cardápio.
        List<Prato> BuscaPorCategoria(string categoria);

        // Retorna apenas os pratos que estão ativos (disponíveis no cardápio).
        List<Prato> BuscaPratosAtivos();

        // Adiciona um novo prato ao cardápio.
        void AdicionaPrato(Prato prato);

        // Atualiza as informações de um prato já existente.
        void AtualizaPrato(Prato prato);

        // Remove um prato do cardápio pelo Id.
        void DeletaPrato(int id);
    }
}
