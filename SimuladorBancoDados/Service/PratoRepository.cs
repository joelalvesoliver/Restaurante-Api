using Microsoft.Extensions.Logging;
using SimuladorBancoDados.Entidade;
using SimuladorBancoDados.Interfaces;

namespace SimuladorBancoDados.Service
{
    public class PratoRepository : IPratoRepository
    {
        private readonly ILogger<PratoRepository> _logger;
        private List<Prato> pratos;
        private int proximoId = 1;
        public PratoRepository(ILogger<PratoRepository> logger)
        {
            _logger = logger;
            pratos = new List<Prato>();
        }
        public void AdicionaPrato(Prato prato)
        {
            prato.Id = proximoId;
            pratos.Add(prato);
            _logger.LogInformation("Prato {0} criado", prato.Id);
            proximoId++;
        }

        public void AtualizaPrato(Prato prato)
        {
            foreach (var item in pratos)
            {
                if (item.Id == prato.Id)
                {
                    pratos.Remove(item);
                    pratos.Add(prato);
                    break;
                }
            }
        }

        public List<Prato> BuscaPorCategoria(string categoria)
        {
            List<Prato> pratosRetorno = new List<Prato>();
            foreach (var item in pratos)
            {
                if (string.Equals(item.Categoria, categoria, StringComparison.CurrentCultureIgnoreCase))
                {
                    pratosRetorno.Add(item);
                }
            }

            return pratosRetorno;
        }

        public Prato? BuscaPratoPeloId(int id)
        {
            foreach (var item in pratos)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        public List<Prato> BuscaPratosAtivos()
        {
            List<Prato> pratosRetorno = new List<Prato>();
            foreach (var item in pratos)
            {
                if (item.Ativo)
                {
                    pratosRetorno.Add(item);
                }
            }

            return pratosRetorno;
        }

        public List<Prato> BuscaTodosPratos()
        {
            return pratos;
        }

        public void DeletaPrato(int id)
        {
            foreach (var item in pratos)
            {
                if (item.Id == id)
                {
                    pratos.Remove(item);
                }
            }
        }

        public void Salvar(Prato prato)
        {
            // Se não tiver Id, gera um novo
            if (prato.Id == 0)
            {
                prato.Id = pratos.Count > 0 ? pratos.Max(p => p.Id) + 1 : 1;
            }

            pratos.Add(prato);
        }

    }
}
