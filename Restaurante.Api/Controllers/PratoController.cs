using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Restaurante.Api.DTOs;
using SimuladorBancoDados;
using SimuladorBancoDados.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Restaurante.Api.Controllers
{
    public class Prato
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome e obrigatorio")]
        [StringLength(150, MinimumLength = 3,
        ErrorMessage = "Nome deve ter entre 3 e 150 caracteres")]
        public string Nome { get; set; }              // Obrigatório, máx 150 caracteres

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal Preco { get; set; }            // Obrigatório, > 0

        [StringLength(500)]
        public string? Descricao { get; set; }         // Máx 500 caracteres (opcional)

        [StringLength(50)]
        public string? Categoria { get; set; }         // Máx 50 caracteres (opcional)
        public string IdFoto { get; set; }            // Obrigatório (ID do arquivo)
        public bool Ativo { get; set; }               // Padrão: true
        public DateTime DataCadastro { get; set; }    // Padrão: DateTime.Now 


        public Prato() { }


        public Prato(int id, string nome, string? descricao, decimal preco, string? categoria)
        {
            Id = id;
            Nome = nome;
            Preco = preco;
            Descricao = descricao;
            Categoria = categoria;
            //IdFoto = idfoto;
            //Ativo = ativo;

        }
    }
}


