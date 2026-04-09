using Restaurante.Api.Filtros;
using Restaurante.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<VerificarAutorizacaoFazerPedido>();
builder.Services.AddScoped<LogAuditoria>();
builder.Services.AddScoped<EnvolveRespostaFilter>();
builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddScoped<VericarCacheFilter>();

builder.Services.AddControllers(
options =>
{
    options.Filters.Add<EnvolveRespostaFilter>();
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