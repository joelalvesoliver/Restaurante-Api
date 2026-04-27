using SimuladorBancoDados.Entidade;

namespace SimuladorBancoDados.Interfaces
{
    public interface IPratoRepository
    {
        Prato? BuscaPratoPeloId(int id);
        List<Prato> BuscaTodosPratos();
        List<Prato> BuscaPorCategoria(string categoria);
        List<Prato> BuscaPratosAtivos();
        void AdicionaPrato(Prato prato);
        void AtualizaPrato(Prato prato);
        void DeletaPrato(int id);

        // novo método para salvar prato com id atrelada ao novo nome de arquivo guid
        void Salvar(Prato prato);
    }
}
