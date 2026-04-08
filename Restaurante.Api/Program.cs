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

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();


// meu primeiro middleware = traceId disponível em todo o pipeline
app.UseMiddleware<RequestTrackingMiddleware>();

// Middleware de cabeçalho obrigatório (X-Turma)
// primeiro verifica se a requisição contém o cabeçalho X-Turma
app.UseMiddleware<RequiredTurmaHeaderMiddleware>();

// meu segundo middleware = Valida o cabeçalho X-Canal apenas para o endpoint /api/Cardapio
// depois valida
app.UseMiddleware<BloqueioHeaderMiddleware>();

// Interrompe apenas POST /api/pedidos fora do horário permitido
app.UseMiddleware<HorarioPedidoMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "menu",
    pattern: "menu/{action=Index}/{id?}",
    defaults: new {controller = "Cardapio" }
    );

app.UseMiddleware<MiddlewareA>();
app.UseMiddleware<MiddlewareB>();
app.UseMiddleware<MiddlewareC>();

app.MapControllers();

app.Run();




/* a sequência de execução das middlewares é determinada pela dependência da resposta de uma
 em relação à seguinte, portanto, a última executada é primeira a receber o retorno e as demais,
uma a uma até fechar a primeira */