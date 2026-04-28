using System;

namespace Restaurante.Api.DTOs

{
    public class PratoResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string IdFoto { get; set; } = string.Empty;
        public string Ur1Download { get; set; } = string.Empty;                     
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
    }

}
