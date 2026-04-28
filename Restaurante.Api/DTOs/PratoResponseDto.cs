using SimuladorBancoDados;
using System.ComponentModel.DataAnnotations;
using Restaurante.Api.DTOs;
namespace Restaurante.Api.DTOs
{
    public class PratoResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public string Descricao { get; set; }
        public string Categoria { get; set; }
        public DateTime DataCadastro { get; set; } // Referente ao seu campo dataDateCadastro
        public string IdFoto { get; set; }
        public string UrlDownloadFoto { get; set; }
    }
}