using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Restaurante.Api.Filtros;
using Restaurante.Api.Middlewares;
using Restaurante.Api.Services;
using SimuladorBancoDados.Interfaces;
using SimuladorBancoDados.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddScoped<VerificarAutorizacaoFazerPedido>();
builder.Services.AddScoped<LogAuditoria>();
builder.Services.AddScoped<EnvolveRespostaFilter>();
builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddScoped<VericarCacheFilter>();
builder.Services.AddScoped<ArquivoService>();
builder.Services.AddScoped<ArquivoService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<ArquivoService>>();
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new ArquivoService(logger, env);
});

//builder.Services.AddSingleton -- o gerenciamento ele é feito no iniciar da aplicação
//builder.Services.AddTransient -- Sempre que é preciso do objeto ele é criado e devolvido
//builder.Services.AddScoped   -- O objeto permanece valido durante o escopo de onde ele foi criado

builder.Services.AddSingleton<IBancoDados, BancoDadosService>();
builder.Services.AddSingleton<IPratoRepository, PratoRepository>();

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

builder.Services.AddControllers(options =>
{
    //options.Filters.Add<EnvolveRespostaFilter>();
    options.Filters.Add<ExceptionFilter>();

}
).AddXmlSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Use: Bearer {seu_token_jwt}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

// meu primeiro middleware
app.UseMiddleware<RequestTrackingMiddleware>();

// meu segundo middleware
app.UseMiddleware<BloqueioHeaderMiddleware>();

app.UseRouting();

//app.UseCors("AllowAllPolicy");
//app.UseCors("FrontendPolicy");

app.UseAuthentication();
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
// CORS Cros-Origin Resource Sharing