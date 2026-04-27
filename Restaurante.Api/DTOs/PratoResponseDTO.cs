namespace Restaurante.Api.DTOs
{
    public class PratoResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public string Descricao { get; set; }
        public string Categoria { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public string IdFoto { get; set; }
        public string UrlDownload { get; set; }
    }
}
