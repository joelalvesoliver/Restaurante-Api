using Restaurante.Api.DTOs;

namespace Restaurante.Api.DTOs
{

    public class CardapioExibicaoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public string Descricao { get; set; }
        public string Categoria { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        // Aqui injetamos o DTO de arquivo que você já possui
        public ArquivoResponseDTO Foto { get; set; }
    }
}