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



// meu segundo middleware
app.UseMiddleware<BloqueioHeaderMiddleware>();
// Configure the HTTP request pipeline.

// meu primeiro middleware
app.UseMiddleware<RequestTrackingMiddleware>();
app.UseMiddleware<BloqueioHorarioMiddleware>();


//Cada middleware:

//Executa um código antes do _next()
//Entrega a execução para o próximo middleware
//Só retoma a execução quando o próximo middleware termina

app.UseMiddleware<MiddlewareA>();
app.UseMiddleware<MiddlewareB>();
app.UseMiddleware<MiddlewareC>();

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