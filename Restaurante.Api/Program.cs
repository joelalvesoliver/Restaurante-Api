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
builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddScoped<VericarCacheFilter>();

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
    options.Filters.Add<ExceptionFilter>();

}
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



// meu segundo middleware
app.UseMiddleware<BloqueioHeaderMiddleware>();
// Configure the HTTP request pipeline.

// meu primeiro middleware
app.UseMiddleware<RequestTrackingMiddleware>();



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