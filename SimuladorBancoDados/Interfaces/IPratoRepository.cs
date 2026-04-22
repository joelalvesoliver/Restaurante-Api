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
    }
}
