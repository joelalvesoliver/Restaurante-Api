using Restaurante.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

//Exercicio 3 - LogMiddleware1
app.UseMiddleware<RequestLogMiddleware1>();

//Exercicio 1 - Middleware de cabecalho obrigatorio
app.UseMiddleware<CabecalhoObrigatorioMiddleware>();

//Exercicio 2 - Curto-circuito por horario
app.UseMiddleware<CurtoCircuitoHorarioMiddleware>();

//Exercicio 3 - LogMiddleware2
app.UseMiddleware<RequestLogMiddleware2>();

//Exercicio 3 - LogMiddleware3
app.UseMiddleware<RequestLogMiddleware3>();


/*
* Middlewares feitos em sala.
/
 // meu segundo middleware
app.UseMiddleware<BloqueioHeaderMiddleware>();
// Configure the HTTP request pipeline.

// meu primeiro middleware
app.UseMiddleware<RequestTrackingMiddleware>();
*/

app.UseAuthorization();

app.MapControllerRoute(
    name: "menu",
    pattern: "menu/{action=Index}/{id?}",
    defaults: new {controller = "Cardapio" }
    );

app.MapControllers();

app.Run();
