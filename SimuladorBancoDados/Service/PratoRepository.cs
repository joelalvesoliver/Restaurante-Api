using Microsoft.Extensions.Logging;
using SimuladorBancoDados.Entidade;
using SimuladorBancoDados.Interfaces;

namespace SimuladorBancoDados.Service
{
    public class PratoRepository : IPratoRepository
    {
        private readonly ILogger<PratoRepository> _logger;
        private List<Prato> pratos;
        public int proximoId { get; private set; } = 1;
        public PratoRepository(ILogger<PratoRepository> logger)
        {
            _logger = logger;
            pratos = new List<Prato>();

            var prato = new Prato
            {
                Id = proximoId,
                Nome = "Prato1",
                Preco = 10m,
                Descricao = "Prato muito gostoso",
                Categoria = "Prato Fino",
                IdFoto = "4d40e340-fe93-46e4-a8f9-f1e22fe11196.jpg",
                UrlDownload = "/api/arquivos/download/4d40e340-fe93-46e4-a8f9-f1e22fe11196.jpg",
                Ativo = true,
                DataCadastro = DateTime.Now
            };
            this.AdicionaPrato(prato);

        }
        public Prato AdicionaPrato(Prato prato)
        {
            prato.Id = proximoId;
            pratos.Add(prato);
            _logger.LogInformation("Prato {0} criado", prato.Id);
            proximoId++;
            return prato;
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
    }
}
