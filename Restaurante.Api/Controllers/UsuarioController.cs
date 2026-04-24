// Importações de ferramentas necessárias para este controller.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Restaurante.Api.DTOs;
using SimuladorBancoDados;
using SimuladorBancoDados.Interfaces;
// Ferramentas para criar e validar tokens JWT.
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Restaurante.Api.Controllers
{
    // Controller responsável por todas as operações relacionadas a usuários (funcionários):
    // criar, buscar, listar, login e geração de token JWT.
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        // IBancoDados: interface do "banco de dados" simulado.
        // Usamos a interface (não a implementação direta) para seguir o princípio de
        // Inversão de Dependência — o controller não sabe como os dados são armazenados.
        private readonly IBancoDados _service;

        // IConfiguration: acesso às configurações do arquivo appsettings.json.
        // Usamos para ler as configurações do JWT (chave secreta, emissor, validade).
        private readonly IConfiguration _configuration;

        // Logger para registrar eventos importantes durante o processamento das requisições.
        private readonly ILogger<UsuarioController> _logger;

        // Construtor: o .NET injeta automaticamente as três dependências ao criar o controller.
        public UsuarioController(IBancoDados service,
                                 IConfiguration configuration,
                                 ILogger<UsuarioController> logger) {
            
            _configuration = configuration;
            _service = service;
            _logger = logger;
        }

        // POST api/usuario
        // Cria um novo funcionário no sistema.
        //
        // [FromBody]: os dados do funcionário vêm no CORPO da requisição (JSON).
        //   Ex: { "nome": "João", "email": "joao@rest.com", "senha": "123456", "funcao": 2 }
        //
        // [FromHeader(Name = "X-Perfil")]: este dado vem no CABEÇALHO da requisição.
        //   O cliente precisa enviar o header "X-Perfil: Gerente" para criar usuários.
        //   Isso demonstra que nem todos os dados precisam vir no corpo da requisição.
        //
        // Por que usar um header para o perfil?
        // Simula uma autorização simples: só quem tem o perfil "Gerente" pode criar funcionários.
        // Em produção, isso seria validado via JWT token — mas aqui é um exemplo didático.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<FuncionarioRespostaDto> CriarUsuario(
            [FromBody] FuncionarioDTO funcionario,
            [FromHeader(Name = "X-Perfil")] string perfil)
        {
            // LogDebug: nível de log mais detalhado, usado para desenvolvimento/debug.
            // Em produção, logs de Debug geralmente são desativados para não poluir os logs.
            // Níveis de log (do menos para o mais grave): Debug → Info → Warning → Error → Critical
            _logger.LogDebug("[CriarUsuario] Iniciando a criação de meu usuário {0},{1}\", funcionario.Nome, funcionario.Email");

            // Valida se o header X-Perfil foi enviado na requisição.
            if (string.IsNullOrEmpty(perfil))
            {
                // 403 Forbidden: autenticado, mas sem permissão para esta ação.
                // StatusCode() permite retornar qualquer código HTTP com um corpo customizado.
                return StatusCode(StatusCodes.Status403Forbidden,
                    new
                    {
                        sucesso = false,
                        codigo = StatusCodes.Status403Forbidden,
                        mensagem = "Header X-Perfil não informado."
                    });
            }

            // Verifica se o perfil é "Gerente" (ignorando maiúsculas/minúsculas).
            // StringComparison.OrdinalIgnoreCase: compara sem diferenciar "gerente" de "Gerente".
            if (!string.Equals(perfil, "Gerente", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new
                    {
                        sucesso = false,
                        codigo = StatusCodes.Status403Forbidden,
                        mensagem = "Apenas gerente tem acesso para cadastrar novos funcionarios!"
                    });
            }

            // ModelState.IsValid: verifica se os dados recebidos passam nas Data Annotations do DTO.
            // Ex: [Required], [EmailAddress], [StringLength] definidos no FuncionarioDTO.
            // O .NET valida automaticamente e armazena os erros em ModelState.
            if (!ModelState.IsValid)
            {
                // Retorna 400 com todos os erros de validação encontrados.
                return BadRequest(ModelState);
            }

            // Monta o objeto de resposta (DTO de saída) com os dados do funcionário criado.
            var resposta = new FuncionarioRespostaDto
            {
                Id = 1,  // Em um banco real, o Id seria gerado automaticamente.
                Nome = funcionario.Nome,
                Email = funcionario.Email,
                Funcao = funcionario.Funcao,
                Ativo = true,
                DataCadastro = DateTime.Now,
            };

            // Salva o novo usuário no banco de dados simulado.
            // Note que criamos um novo UsuarioDto para não passar o FuncionarioDTO diretamente,
            // mantendo as camadas separadas (DTO de entrada ≠ entidade do banco).
            _service.AdicionarNovoUsuario(new UsuarioDto
            {
                Nome = funcionario.Nome,
                Email = funcionario.Email,
                Funcao = funcionario.Funcao,
                DataCadastro = DateTime.Now,
                Ativo = true,
                Senha = funcionario.Senha
            });
            
            // LogInformation: registra eventos normais e importantes do sistema.
            _logger.LogInformation("[CriarUsuario] Finalizado o cadastro do usuario {0},{1}", funcionario.Nome, funcionario.Email);

            // CreatedAtRoute: retorna 201 Created com o header "Location" apontando para a URL
            // onde o novo recurso pode ser encontrado. O cliente sabe onde buscar o que foi criado.
            // "BuscarUsuario" é o nome da rota de GET definida abaixo (Name = "BuscarUsuario").
            return CreatedAtRoute("BuscarUsuario", new { nome = resposta.Nome }, resposta);
        }


        // GET api/usuario/{nome}
        // Busca um usuário pelo nome. Esta rota tem um nome ("BuscarUsuario") para que
        // outros endpoints possam referenciá-la (como o CreatedAtRoute acima).
        //
        // Name = "BuscarUsuario": nomeia esta rota para ser referenciada em CreatedAtRoute.
        [HttpGet("{nome}", Name = "BuscarUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FuncionarioRespostaDto> BuscarUsuario(string nome)
        {
            var usuario = _service.BuscarUsuarioPeloNome(nome);
            if(usuario == null)
            {
                // 404 Not Found: nenhum usuário com esse nome foi encontrado.
                return NotFound(
                new
                {
                    mensagem = "Usuario não encontrado!",
                });
            }

            return Ok(usuario);
        }

        // GET api/usuario?nome=João&email=joao@rest.com
        // Lista usuários com filtros opcionais por nome e/ou email.
        //
        // [HttpGet] sem parâmetro de rota: responde ao GET na URL base do controller.
        // [FromQuery]: os filtros são passados como query string na URL.
        //   Ex: /api/usuario?nome=Maria  ou  /api/usuario?email=maria@ada.com
        //   O "?" após o tipo (string?) indica que o parâmetro é opcional (pode ser null).
        //
        // [Authorize(Roles = "Gerente, Admin")]:
        //   Protege este endpoint — apenas usuários autenticados com o JWT token
        //   E que tenham o role "Gerente" ou "Admin" podem acessá-lo.
        //   Se o token não for enviado → 401 Unauthorized.
        //   Se o token for válido mas o role não bater → 403 Forbidden.
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Gerente, Admin")]
        public ActionResult<FuncionarioRespostaDto> ListarUsuarios(
            [FromQuery] string? nome, [FromQuery] string? email)
        {
            // Lógica de filtros combinados: o método verifica quais filtros foram enviados
            // e aplica a busca mais específica possível.

            // Filtro por nome E email ao mesmo tempo
            if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(email))
            {
                var funcionario = _service.BuscarUsuarioPorNomeEEmail(nome, email);
                return Ok(funcionario);
            }

            // Filtro apenas por nome
            if (!string.IsNullOrEmpty(nome))
            {
                var usuario = _service.BuscarUsuarioPeloNome(nome);
                return Ok(usuario);
            }

            // Filtro apenas por email
            if (!string.IsNullOrEmpty(email))
            {
                var usuario = _service.BuscarUsuarioPorEmail(email);
                return Ok(usuario);
            }

            // Sem filtros: retorna todos os usuários
            var funcionarios = _service.ListarUsuarios();
            return Ok(funcionarios);
        }

        // POST api/usuario/usuarioXml/garcom
        // Cria um novo garçom enviando os dados em formato XML (não JSON).
        //
        // [Consumes("application/xml")]: este endpoint aceita APENAS XML como entrada.
        // O .NET identifica o formato pelo header "Content-Type: application/xml" da requisição.
        // Exemplo de XML esperado:
        //   <Usuario><Nome>João</Nome><Email>joao@rest.com</Email></Usuario>
        //
        // Por que mostrar XML? Para demonstrar que APIs podem suportar múltiplos formatos.
        // Na prática, JSON é mais comum, mas XML ainda é usado em integrações legadas.
        [HttpPost("usuarioXml/garcom")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Consumes("application/xml")]
        public ActionResult<FuncionarioRespostaDto> CriaUsuarioXml([FromBody] UsuarioXmlDto dto)
        {
            if(dto == null)
                return BadRequest("XML Inválido");

            // Cria o usuário sempre como Garçom, independente do que foi enviado.
            // A rota já define explicitamente que é para cadastrar garçons.
            _service.AdicionarNovoUsuario(new UsuarioDto
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Funcao = Funcao.Garcom,
                DataCadastro = DateTime.Now,
                Ativo = true,
                Senha = null  // Garçons criados via XML não têm senha definida neste fluxo
            });

            var resposta = new FuncionarioRespostaDto
            {
                Id = 1,
                Nome = dto.Nome,
                Email = dto.Email,
                Funcao = Funcao.Garcom,
                Ativo = true,
                DataCadastro = DateTime.Now,
            };

            return CreatedAtRoute("BuscarUsuario", new { nome = resposta.Nome }, resposta);
        }

        // POST api/usuario/login
        // Autentica um usuário e retorna um JWT token se as credenciais estiverem corretas.
        //
        // O que é JWT (JSON Web Token)?
        // É um padrão para transmitir informações de forma segura entre sistemas.
        // Após o login, o servidor gera um token assinado digitalmente.
        // O cliente armazena este token e o envia em toda requisição protegida
        // no header "Authorization: Bearer {token}".
        // O servidor verifica a assinatura do token sem precisar consultar o banco.
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<FuncionarioRespostaDto> Login([FromBody] LoginDTO dto)
        {
            // Valida as Data Annotations do LoginDTO ([Required], [EmailAddress]).
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Busca o usuário pelo e-mail no banco de dados.
            var usuario = _service.BuscarUsuarioPorEmail(dto.Email);
            if(usuario == null)
            {
                // 401 Unauthorized: credenciais inválidas.
                // Por segurança, não informamos se o problema é o email ou a senha,
                // pois isso ajudaria um atacante a descobrir quais emails estão cadastrados.
                return Unauthorized(new 
                { 
                    mensagem = "Email ou senha inválidos"
                });
            }

            // Verifica se a senha informada corresponde ao usuário encontrado.
            var autenticado = _service.AutenticarUsuario(dto.Email, dto.Senha);
            if (!autenticado)
            {
                return Unauthorized(new
                {
                    mensagem = "Email ou senha inválidos"
                });
            }

            // Login válido! Gera o token JWT para o usuário.
            // "out DateTime expiracao" é um parâmetro de saída: o método GerarToken
            // retorna o token E preenche a variável "expiracao" ao mesmo tempo.
            var token = GerarToken(usuario, out DateTime expiracao);

            // Retorna os dados do usuário, o token JWT e quando ele expira.
            return Ok( new
            {
                usuario = usuario,
                token = token,
                expiracao = expiracao
            });
        }

        // Método privado que gera o JWT token para o usuário autenticado.
        // "private": só pode ser chamado dentro desta classe.
        // "out DateTime expiracao": parâmetro de saída — além de retornar o token (string),
        // este método também "retorna" a data de expiração através deste parâmetro.
        private string GerarToken(UsuarioDto usuario, out DateTime expiracao)
        {
            // Lê as configurações de JWT do arquivo appsettings.json.
            // GetSection("Jwt") acessa o objeto "Jwt" no JSON de configuração.
            var jwtSection = _configuration.GetSection("Jwt");

            var jwtKey = jwtSection["Key"];          // Chave secreta para assinar o token
            var jwtIssuer = jwtSection["Issuer"];    // Quem emitiu o token (o servidor)
            var jwtAudience = jwtSection["Audience"]; // Para quem o token é destinado (o cliente)
            // Tenta converter ExpiresInMinutes para int; usa 120 minutos como padrão se falhar.
            var expiraEmMinutos = int.TryParse(jwtSection["ExpiresInMinutes"], out var valor)
                ? valor
                : 120;

            // Converte o enum Funcao para string de forma segura.
            // Enum.IsDefined verifica se o valor é um valor válido do enum antes de converter.
            var funcao = Enum.IsDefined(typeof(Funcao), usuario.Funcao) 
                ? ((Funcao)usuario.Funcao).ToString() : usuario.Funcao.ToString();
            
            // CLAIMS: informações embutidas dentro do token JWT.
            // Claims são pares chave-valor que identificam o usuário e suas permissões.
            // O servidor pode verificar qualquer claim sem consultar o banco de dados.
            var claims = new List<Claim>();

            // Sub (Subject): identificador único do usuário (geralmente o Id).
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()));
            // Email do usuário embutido no token.
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, usuario.Email.ToString()));
            // Nome do usuário — usado pelo ClaimTypes.Name para identificação.
            claims.Add(new Claim(ClaimTypes.Name, usuario.Nome));
            // Role (função/cargo): usado pelo [Authorize(Roles = "...")] nos endpoints.
            // Ex: "Gerente" → permite acesso a endpoints protegidos por [Authorize(Roles = "Gerente")].
            claims.Add(new Claim(ClaimTypes.Role, funcao));
            // Jti (JWT ID): identificador único deste token específico.
            // Útil para revogar tokens individualmente se necessário.
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            // SymmetricSecurityKey: cria a chave de assinatura a partir do texto secreto.
            // Encoding.UTF8.GetBytes converte a string para bytes (necessário para criptografia).
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            // SigningCredentials: define o algoritmo de assinatura (HmacSha256 = HMAC com SHA-256).
            // A assinatura garante que o token não foi alterado após ser emitido.
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            // Define a data de expiração do token.
            // Após esta data, o token não é mais aceito pelo servidor.
            expiracao = DateTime.Now.AddMinutes(expiraEmMinutos);

            // JwtSecurityToken: monta o token JWT com todos os componentes.
            // Um JWT tem 3 partes: Header.Payload.Signature
            var tokenJwt = new JwtSecurityToken(
                issuer: jwtIssuer,           // Quem emitiu
                audience: jwtAudience,       // Para quem
                claims: claims,              // Informações do usuário
                expires: expiracao,          // Quando expira
                signingCredentials: credenciais  // Assinatura digital
            );

            // WriteToken: converte o objeto JwtSecurityToken para a string final do token.
            // Formato: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIn0.xxxx"
            return new JwtSecurityTokenHandler().WriteToken(tokenJwt);
        }
    }
}
