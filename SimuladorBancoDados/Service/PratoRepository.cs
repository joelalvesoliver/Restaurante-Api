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
            // Lista de nomes para popular rapidamente
            var nomes = new List<string>
    {
        "Parmegiana de Frango", "Filé Oswaldo Aranha", "Sushi Loko Combo", "Yakisoba Clássico",
        "Lula à Milanesa", "Strogonoff de Carne", "Alcatra à Moda da Casa", "Feijoada Completa",
        "Salada Caesar", "Nhoque da Fortuna", "Risoto de Cogumelos", "Bacalhau à Brás",
        "Hambúrguer Gourmet", "Pizza Margherita", "Lasanha Bolonhesa", "Taco de Chilli",
        "Camarão na Moranga", "Frango Xadrez", "Peixe Frito", "Costelinha BBQ",
        "Penne à Carbonara", "Sopa de Cebola", "Moqueca Capixaba", "Pão com Linguiça", "Petit Gâteau"
    };

            var categorias = new[] { "Carnes", "Peixes", "Massas", "Japonesa", "Sobremesas", "Saladas" };
            var random = new Random();

            foreach (var nome in nomes)
            {
                pratos.Add(new Prato
                {
                    Id = proximoId++,
                    Nome = nome,
                    Preco = (decimal)(random.NextDouble() * (80 - 20) + 20), // Preço aleatório entre 20 e 80
                    Descricao = $"Delicioso {nome} preparado com ingredientes selecionados pelo chef.",
                    Categoria = categorias[random.Next(categorias.Length)],
                    IdFoto = $"foto_{nome.Replace(" ", "_").ToLower()}.jpg",
                    Ativo = true,
                    DataCadastro = DateTime.Now
                });
            }

            _logger.LogInformation("{0} pratos foram populados no repositório.", pratos.Count);
        


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
    }
}
