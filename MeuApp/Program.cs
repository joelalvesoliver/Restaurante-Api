// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

using Restaurante.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o filtro ao container de serviços
builder.Services.AddScoped<ValidatePositiveIdFilter>();

var app = builder.Build();

app.MapControllers();

app.Run();

// Registrar o Filtro Globalmente

using Restaurante.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o filtro globalmente
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AdicionarHeaderProcessadoEmFilter>();
});

var app = builder.Build();

app.MapControllers();

app.Run();

[HttpGet("exemplo")]
public IActionResult Exemplo()
{
    throw new ArgumentException("O argumento fornecido é inválido.");
}