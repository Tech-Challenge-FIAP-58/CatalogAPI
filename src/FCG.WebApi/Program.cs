using FCG.Infra.Context;
using FCG.Infra.Mapping;
using FCG.WebApi.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(configSection);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(
    cfg => { },
    AppDomain.CurrentDomain.GetAssemblies()
);

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CatalogProfile>();
    cfg.AddProfile<OrderProfile>();
    cfg.AddProfile<GameProfile>();
});

builder.RegisterServices();

builder.AddMessageBusConfiguration();

builder.InitilizeRetrySettings();

builder.AddMassTransitSettings();

builder.AddJwtConfig();

var app = builder.Build();

#region MIGRATION COM RETRY
// Observação: este bloco roda **antes** do servidor iniciar. Ele tenta aplicar
// migrations até 'maxAttempts' vezes, com backoff exponencial (limitado).
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    const int maxAttempts = 10;
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            logger.LogInformation("Tentando aplicar migrations (tentativa {Attempt}/{MaxAttempts})...", attempt, maxAttempts);
            dbContext.Database.Migrate(); // aplica migrations pendentes (síncrono)
            logger.LogInformation("Migrations aplicadas com sucesso.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao aplicar migrations na tentativa {Attempt}.", attempt);
            if (attempt == maxAttempts)
            {
                logger.LogError(ex, "Não foi possível aplicar migrations após {MaxAttempts} tentativas. Encerrando aplicação.", maxAttempts);
                throw; // aborta a inicialização (você pode optar por não lançar e continuar)
            }
            // backoff simples (2s * attempt), limitado a 30s
            var delay = TimeSpan.FromSeconds(Math.Min(30, 2 * attempt));
            logger.LogInformation("Aguardando {Delay} antes da próxima tentativa...", delay);
            // usa Task.Delay para não bloquear a thread
            await Task.Delay(delay);
        }
    }
}
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok("Healthy")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
