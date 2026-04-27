using System;

namespace SimuladorBancoDados.Entidade
{
    internal class Usuario
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Senha { get; private set; }

        public Funcao Funcao { get; private set; }

        public bool Ativo { get; private set; }

        public DateTime DataCadastro { get; private set; }

        public Usuario(UsuarioDto usuarioDto, int id)
        {
            Id = id;
            Nome = usuarioDto.Nome;
            Email = usuarioDto.Email;
            Senha = usuarioDto.Senha;
            Funcao = usuarioDto.Funcao;
            Ativo = usuarioDto.Ativo;
            DataCadastro = usuarioDto.DataCadastro;
        }
    }
}
