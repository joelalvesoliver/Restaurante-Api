using Restaurante.Api.Middlewares;
using Restaurante.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// AULA_03 - Registro de filtros
// ===============================
builder.Services.AddControllers(options =>
{
    // Exercício 2 (AULA_03):
    // Filtro de RESULTADO que adiciona o header X-Processado-Em
    options.Filters.Add<AdicionarHeaderProcessadoEmFilter>();

    // Exercício 3 (AULA_03):
    // Filtro de EXCEÇÃO para tratar ArgumentException e retornar 422 (Unprocessable Entity)
    // (ADICIONADO agora para concluir o Exercício 3)
    options.Filters.Add<ArgumentExceptionFilter>();
});

// Exercício 1 (AULA_03):
// Registro do filtro de ação para uso com [ServiceFilter]
builder.Services.AddScoped<ValidarIdPositivoFilter>();

// Registro do filtro de RESULTADO (global)
builder.Services.AddScoped<AdicionarHeaderProcessadoEmFilter>();

// Exercício 3 (AULA_03):
// Registro do filtro de EXCEÇÃO no DI (necessário porque ele foi adicionado globalmente acima)
// (ADICIONADO agora para concluir o Exercício 3)
builder.Services.AddScoped<ArgumentExceptionFilter>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===============================
// PIPELINE HTTP (OBRIGATÓRIO)
// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// DESATIVADO no ambiente corporativo
// app.UseHttpsRedirection();

app.UseMiddleware<BloqueioHeaderMiddleware>();
app.UseMiddleware<RequestTrackingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();