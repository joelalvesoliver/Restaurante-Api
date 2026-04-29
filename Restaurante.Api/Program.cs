using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Restaurante.Api.Filtros;
using Restaurante.Api.Middlewares;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<VerificarAutorizacaoFazerPedido>();
builder.Services.AddScoped<LogAuditoria>();
builder.Services.AddScoped<EnvolveRespostaFilter>();
//builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddScoped<VericarCacheFilter>();
//exercicio 1 Aula 3
builder.Services.AddScoped<ValidaParametroRotaID>();
//exercicio 2 Aula 3
builder.Services.AddScoped<AdicionaHeaderXProcessado>();
// Exercico 3 Aula 3
builder.Services.AddScoped<ExceptionFilterArgument>();

//builder.Services.AddSingleton -- o gerenciamento ele é feito no iniciar da aplicação
//builder.Services.AddTransient -- Sempre que é preciso do objeto ele é criado e devolvido
//builder.Services.AddScoped   -- O objeto permanece valido durante o escopo de onde ele foi criado

builder.Services.AddSingleton<IBancoDados, BancoDadosService>();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"]
    ?? throw new InvalidOperationException("Configuracao Jwt:Key nao encontrada.");
var jwtIssuer = jwtSection["Issuer"]
    ?? throw new InvalidOperationException("Configuracao Jwt:Issuer nao encontrada.");
var jwtAudience = jwtSection["Audience"]
    ?? throw new InvalidOperationException("Configuracao Jwt:Audience nao encontrada.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddControllers(
options =>
{
    //options.Filters.Add<EnvolveRespostaFilter>();
    //options.Filters.Add<ExceptionFilter>();


    //exercicio 1 Aula 3
    //options.Filters.Add<ValidaParametroRotaID>();
    //exercicio 2 Aula 3
    options.Filters.Add<AdicionaHeaderXProcessado>();
    //exercicio 3 Aula 3
    options.Filters.Add<ExceptionFilterArgument>();
}
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilterArgument>();
});*/

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//MiddleWare do Exercicio 2 (exercicio 3 está resolvido dentro dos micclewares dos exercicios 1,2 e o "segundo middleware")
//app.UseMiddleware<BloqueiaHorarioMiddleware>();



// Middleware do Exercicio 1
//app.UseMiddleware<BloqueioHeader400Middleware>();

// meu segundo middleware

//app.UseMiddleware<BloqueioHeaderMiddleware>();

// Configure the HTTP request pipeline.

// meu primeiro middleware
//app.UseMiddleware<RequestTrackingMiddleware>();



app.UseAuthorization();

app.MapControllerRoute(
    name: "menu",
    pattern: "menu/{action=Index}/{id?}",
    defaults: new {controller = "Cardapio" }
    );

app.MapControllers();

app.Run();




// Enviar E-mail da conta do Github no Chat
// Para adicionar vocês como colaboradores do projeto