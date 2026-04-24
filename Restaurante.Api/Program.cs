// ============================================================
// Program.cs — Ponto de entrada da aplicação ASP.NET Core
// ============================================================
// Este é o arquivo mais importante da aplicação: é aqui que tudo começa.
// Ele configura todos os serviços, middlewares e inicia o servidor HTTP.
//
// No .NET 6+, o Program.cs usa "Minimal Hosting" — sem a classe Program
// ou o método Main explícitos. O código roda diretamente no arquivo.
// ============================================================

// Importações dos pacotes necessários para este arquivo.
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Restaurante.Api.Filtros;
using Restaurante.Api.Middlewares;
using Restaurante.Api.Services;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Service;
using System.Text;

// ============================================================
// PARTE 1: CRIAÇÃO DO BUILDER
// WebApplication.CreateBuilder cria o "construtor" da aplicação.
// É aqui que configuramos TUDO antes de a aplicação iniciar.
// "args" são argumentos de linha de comando (como --environment Production).
// ============================================================
var builder = WebApplication.CreateBuilder(args);

// ============================================================
// PARTE 2: REGISTRO DE SERVIÇOS (Injeção de Dependência)
// "builder.Services" é o container de injeção de dependência.
// Aqui registramos todas as classes que poderão ser "injetadas"
// automaticamente nos controllers, filtros e outras classes.
// ============================================================

// --- CORS (Cross-Origin Resource Sharing) ---
// CORS controla quais domínios externos podem chamar esta API.
// Exemplo: se o frontend está em "https://meuapp.com", ele precisa
// estar na lista de origens permitidas para chamar esta API.
// As políticas abaixo estão comentadas — ative conforme necessário:

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAllPolicy",
//        policy =>
//        {
//            policy.AllowAnyOrigin()    // Permite qualquer domínio
//                  .AllowAnyMethod()    // Permite GET, POST, PUT, DELETE, etc.
//                  .AllowAnyHeader();   // Permite qualquer cabeçalho
//        });
//});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("FrontendPolicy", policy =>
//    {
//        // Liberar apenas o frontend local da turma
//        policy.WithOrigins("https://lms.ada.tech/", "")
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

// --- Registro dos Filtros ---
// AddScoped: uma nova instância é criada para cada requisição HTTP.
// Perfeito para filtros que precisam de dados específicos de cada requisição.
// Os filtros precisam ser registrados aqui para que o [ServiceFilter] funcione nos controllers.
builder.Services.AddScoped<VerificarAutorizacaoFazerPedido>();
builder.Services.AddScoped<LogAuditoria>();
builder.Services.AddScoped<EnvolveRespostaFilter>();
builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddScoped<VericarCacheFilter>();
builder.Services.AddScoped<ArquivoService>();

// Diferença entre os ciclos de vida dos serviços:
// AddSingleton  → apenas UMA instância durante toda a vida da aplicação.
//                 Todos compartilham a mesma instância. Bom para serviços sem estado.
// AddTransient  → uma nova instância a cada vez que o serviço é solicitado.
//                 Bom para serviços leves e stateless.
// AddScoped     → uma instância por requisição HTTP.
//                 A mesma instância é compartilhada dentro de uma requisição, mas
//                 uma nova é criada para cada requisição diferente.

// BancoDadosService é Singleton porque age como um "banco em memória":
// precisa manter os dados durante toda a vida da aplicação.
// Se fosse Scoped, os dados seriam perdidos a cada requisição!
builder.Services.AddSingleton<IBancoDados, BancoDadosService>();
builder.Services.AddSingleton<IPratoRepository, PratoRepository>();

// ============================================================
// PARTE 3: CONFIGURAÇÃO DE AUTENTICAÇÃO JWT
// ============================================================
// Lê as configurações de JWT do appsettings.json.
// "??" é o operador null-coalescing: se o valor for null, lança uma exceção
// com uma mensagem clara — melhor do que um NullReferenceException genérico.
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"]
    ?? throw new InvalidOperationException("Configuracao Jwt:Key nao encontrada.");
var jwtIssuer = jwtSection["Issuer"]
    ?? throw new InvalidOperationException("Configuracao Jwt:Issuer nao encontrada.");
var jwtAudience = jwtSection["Audience"]
    ?? throw new InvalidOperationException("Configuracao Jwt:Audience nao encontrada.");

// Configura o sistema de autenticação para usar JWT Bearer tokens.
// Após o login, o cliente envia o token no header: "Authorization: Bearer {token}"
// O .NET valida o token automaticamente em cada requisição protegida com [Authorize].
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // TokenValidationParameters: define as regras de validação do token recebido.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,           // Verifica se o token foi emitido por este servidor
            ValidateAudience = true,          // Verifica se o token é destinado a esta API
            ValidateLifetime = true,          // Verifica se o token não está expirado
            ValidateIssuerSigningKey = true,  // Verifica a assinatura digital do token
            ValidIssuer = jwtIssuer,          // Valor esperado para o emissor
            ValidAudience = jwtAudience,      // Valor esperado para o destinatário
            // Reconstrói a chave de assinatura para verificar a autenticidade do token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// ============================================================
// PARTE 4: CONFIGURAÇÃO DOS CONTROLLERS
// ============================================================
builder.Services.AddControllers(options =>
{
    // Filtros globais: aplicados a TODOS os endpoints de TODOS os controllers.
    // ExceptionFilter está ativo globalmente — qualquer exceção não tratada será capturada.
    //options.Filters.Add<EnvolveRespostaFilter>();  // (comentado — ative para envolver todas as respostas)
    options.Filters.Add<ExceptionFilter>();
})
// AddXmlSerializerFormatters: permite que a API receba e envie dados em XML,
// além do JSON padrão. Necessário para o endpoint que aceita "application/xml".
.AddXmlSerializerFormatters();

// ============================================================
// PARTE 5: SWAGGER / OPENAPI
// Swagger é uma ferramenta que gera documentação interativa da API automaticamente.
// Acesse em: http://localhost:{porta}/swagger durante o desenvolvimento.
// ============================================================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Configura o Swagger para suportar autenticação JWT.
    // Isso adiciona o botão "Authorize" na interface do Swagger para inserir o token.
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Use: Bearer {seu_token_jwt}",
        Name = "Authorization",        // Nome do header HTTP
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,  // O token vai no header
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Define que todos os endpoints protegidos precisam do token Bearer.
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ============================================================
// PARTE 6: CONSTRUÇÃO E CONFIGURAÇÃO DO PIPELINE HTTP
// Após o "Build()", não podemos mais registrar novos serviços.
// Agora configuramos os middlewares (a "esteira" de processamento).
// ============================================================
var app = builder.Build();

// Swagger só fica disponível no ambiente de desenvolvimento.
// Em produção, a documentação não é exposta por segurança.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona automaticamente requisições HTTP para HTTPS (mais seguro).
app.UseHttpsRedirection();

// ============================================================
// ORDEM DOS MIDDLEWARES IMPORTA MUITO!
// Cada middleware é executado na ordem em que é registrado.
// A requisição "entra" pelos middlewares de cima para baixo.
// A resposta "volta" de baixo para cima.
// ============================================================

// BloqueioHeaderMiddleware: bloqueia requisições sem o header X-Canal correto
// para rotas do Cardápio. Registrado antes do tracking para bloquear cedo.
app.UseMiddleware<BloqueioHeaderMiddleware>();

// RequestTrackingMiddleware: adiciona um traceId único a cada requisição para rastreamento.
// Registrado aqui para que o traceId esteja disponível para todos os filtros e controllers.
app.UseMiddleware<RequestTrackingMiddleware>();

// Configura o roteamento de URLs para os controllers.
app.UseRouting();

// Ativa as políticas de CORS configuradas acima (descomente e escolha a política):
//app.UseCors("AllowAllPolicy");
//app.UseCors("FrontendPolicy");

// Ativa o sistema de autorização JWT.
// DEVE vir após UseRouting() e antes de MapControllers().
// Verifica o token JWT em requisições para endpoints com [Authorize].
app.UseAuthorization();

// Rota convencional nomeada: mapeia a URL "menu/..." para o CardapioController.
// Demonstra como criar rotas com nomes e valores padrão além dos atributos [Route].
// Ex: GET /menu → CardapioController.Index()
app.MapControllerRoute(
    name: "menu",
    pattern: "menu/{action=Index}/{id?}",
    defaults: new {controller = "Cardapio" }
    );

// Ativa o roteamento baseado em atributos [Route] definidos nos controllers.
// Sem esta linha, os endpoints não seriam acessíveis.
app.MapControllers();

// Inicia o servidor e fica "ouvindo" por requisições.
// A aplicação fica rodando até ser encerrada manualmente.
app.Run();

// CORS (Cross-Origin Resource Sharing):
// Mecanismo de segurança do browser que impede que sites de outros domínios
// façam requisições para esta API sem permissão explícita.
// Configure as políticas de CORS no início deste arquivo conforme necessário.