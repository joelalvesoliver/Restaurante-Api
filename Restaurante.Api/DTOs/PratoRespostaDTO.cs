namespace Restaurante.Api.DTOs
{
    public class PratoRespostaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Descricao { get; set; }
        public string Categoria { get; set; }
        public DateTime DataCadastro { get; set; }
        public string? IdFoto { get; set; }

        public bool Ativo {  get; set; }

        public string UrlDownloadFoto { get; set; } 
    }
}
