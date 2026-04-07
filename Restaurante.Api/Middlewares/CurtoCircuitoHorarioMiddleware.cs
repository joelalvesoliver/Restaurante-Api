public class CurtoCircuitoHorarioMiddleware
{
    private readonly RequestDelegate _next;
    public CurtoCircuitoHorarioMiddleware(RequestDelegate next)
    {
        _next = next;        
    }

    public async Task InvokeAsync(HttpContext Context)
    {
        DateTime DhAcesso = DateTime.Now;
        TimeSpan HoraAcesso = DhAcesso.TimeOfDay;
        TimeSpan PeriodoAcessoInicio = new TimeSpan(0, 0, 0);
        TimeSpan PeriodoAcessoFinal = new TimeSpan(3, 0, 0);

        if (Context.Request.Method == "POST" && Context.Request.Path.StartsWithSegments("/api/Pedidos")){
            if(!(HoraAcesso >= PeriodoAcessoInicio && HoraAcesso <= PeriodoAcessoFinal))
            {
                var traceId = Context.Request.Headers["traceId"].ToString();
                Console.WriteLine($"Reauisição {traceId} - Acesso fora do horário permitido!");
                Context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            } 
        }
        await _next(Context);
    }
}
