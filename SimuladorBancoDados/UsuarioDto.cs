namespace SimuladorBancoDados
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }

        public Funcao Funcao { get; set; }

        public bool Ativo { get; set; }

        public DateTime DataCadastro { get; set; }
    }

    public enum Funcao
    {
        Gerente = 1,
        Garcom = 2,
        Atendente = 3,
        Cozinha = 4
    }
}
