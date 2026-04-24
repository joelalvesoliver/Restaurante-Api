// Importa as ferramentas de logging (registro de eventos) do .NET.
using Microsoft.Extensions.Logging;
using SimuladorBancoDados.Entidade;
using SimuladorBancoDados.Interfaces;

namespace SimuladorBancoDados.Service
{
    // PratoRepository implementa a interface IPratoRepository.
    // Esta classe é responsável por todas as operações de dados relacionadas a Pratos:
    // criar, ler, atualizar e deletar (CRUD - Create, Read, Update, Delete).
    //
    // Assim como BancoDadosService, os dados ficam em uma lista em memória (sem banco real).
    // Em um projeto de produção, esta classe acessaria um banco de dados real
    // via Entity Framework ou outro ORM (Object Relational Mapper).
    public class PratoRepository : IPratoRepository
    {
        // Ferramenta para registrar eventos no log da aplicação.
        // Usamos o logger para registrar quando pratos são criados, por exemplo.
        private readonly ILogger<PratoRepository> _logger;

        // Lista que armazena os pratos em memória (simula a tabela "Pratos" do banco).
        private List<Prato> pratos;

        // Contador para gerar Ids sequenciais automaticamente, como AUTO_INCREMENT no banco.
        private int proximoId = 1;

        // Construtor: recebe o logger por Injeção de Dependência e inicializa a lista vazia.
        public PratoRepository(ILogger<PratoRepository> logger)
        {
            _logger = logger;
            pratos = new List<Prato>();
        }

        // Adiciona um novo prato ao cardápio (simula INSERT no banco de dados).
        // Define o Id do prato com o próximo valor disponível e registra no log.
        public void AdicionaPrato(Prato prato)
        {
            prato.Id = proximoId;
            pratos.Add(prato);
            // Registra no log que um prato foi criado, útil para auditoria e debug.
            _logger.LogInformation("Prato {0} criado", prato.Id);
            proximoId++;
        }

        // Atualiza os dados de um prato existente (simula UPDATE no banco de dados).
        // Estratégia: remove o prato antigo e adiciona o atualizado no lugar.
        public void AtualizaPrato(Prato prato)
        {
            foreach (var item in pratos)
            {
                if (item.Id == prato.Id)
                {
                    // Remove o prato antigo da lista
                    pratos.Remove(item);
                    // Adiciona o prato atualizado
                    pratos.Add(prato);
                    break; // Encontrou e atualizou, sai do loop
                }
            }
        }

        // Busca todos os pratos de uma categoria específica (simula SELECT com WHERE).
        // StringComparison.CurrentCultureIgnoreCase faz a comparação ignorando maiúsculas/minúsculas.
        // Ex: "saladas" e "Saladas" serão tratados como iguais.
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

        // Busca um único prato pelo seu Id (simula SELECT WHERE id = ?).
        // Retorna null se nenhum prato com esse Id existir.
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

        // Retorna apenas os pratos marcados como ativos (disponíveis no cardápio).
        // Pratos inativos não aparecem para o cliente, mas não são deletados do sistema.
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

        // Retorna todos os pratos, ativos e inativos (simula SELECT * FROM Pratos).
        public List<Prato> BuscaTodosPratos()
        {
            return pratos;
        }

        // Remove um prato da lista pelo Id (simula DELETE WHERE id = ?).
        // Atenção: uma vez removido da lista em memória, o prato é perdido permanentemente.
        // Em sistemas reais, geralmente preferimos "desativar" em vez de deletar.
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
