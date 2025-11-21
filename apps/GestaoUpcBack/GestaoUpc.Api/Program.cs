using GestaoUpc.Infra.Ioc;
using GestaoUpc.Domain.Data.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddApiInversionOfControl(builder.Configuration);

builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica migrações automaticamente
await ApplyMigrationsAsync(app);

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GestaoUpc API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Método para aplicar migrações automaticamente
static async Task ApplyMigrationsAsync(WebApplication app)
{
    // Verifica se a aplicação automática de migrações está habilitada
    var autoMigrate = app.Configuration.GetValue<bool>("Database:AutoMigrate", defaultValue: true);
    
    if (!autoMigrate)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Aplicação automática de migrações está desabilitada.");
        return;
    }

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    
    try
    {
        var context = services.GetRequiredService<GestaoUpcDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Verifica se há migrações pendentes
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Aplicando {Count} migração(ões) pendente(s)...", pendingMigrations.Count());
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrações aplicadas com sucesso!");
        }
        else
        {
            logger.LogInformation("Nenhuma migração pendente. Banco de dados está atualizado.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações");
        throw;
    }
}
