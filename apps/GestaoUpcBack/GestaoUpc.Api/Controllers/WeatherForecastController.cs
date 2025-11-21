using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace GestaoUpc.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly string _connectionString;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(IConfiguration configuration, ILogger<WeatherForecastController> logger)
    {
        var connectionString = configuration["DATABASE_PUBLIC_URL"] 
            ?? configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DATABASE_PUBLIC_URL' not found or is empty.");
        }
        
        // Converte URI para connection string se necessário
        _connectionString = ConvertUriToConnectionString(connectionString);
        _logger = logger;
    }

    private static string ConvertUriToConnectionString(string connectionString)
    {
        // Se já está no formato de connection string tradicional, retorna como está
        if (!connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                // Tenta validar como connection string tradicional
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                return builder.ConnectionString;
            }
            catch
            {
                return connectionString;
            }
        }

        // Converte URI para connection string
        try
        {
            var uri = new Uri(connectionString);
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Database = uri.AbsolutePath.TrimStart('/'),
                Username = uri.UserInfo.Split(':')[0],
                Password = uri.UserInfo.Contains(':') ? uri.UserInfo.Split(':')[1] : string.Empty
            };

            return builder.ConnectionString;
        }
        catch
        {
            // Se falhar na conversão, retorna a connection string original
            // O Npgsql pode aceitar o formato URI diretamente
            return connectionString;
        }
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("database-status", Name = "GetDatabaseStatus")]
    public async Task<IActionResult> GetDatabaseStatus()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                _logger.LogError("Connection string está vazia ou nula");
                return StatusCode(503, new
                {
                    status = "offline",
                    message = "Connection string não configurada",
                    timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation("Tentando conectar ao banco de dados...");
            
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            
            // Executa uma query simples para verificar se o banco está respondendo
            await using var command = new NpgsqlCommand("SELECT 1", connection);
            await command.ExecuteScalarAsync();
            
            _logger.LogInformation("Conexão com o banco de dados estabelecida com sucesso");
            
            return Ok(new
            {
                status = "online",
                message = "Banco de dados está online e respondendo",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao conectar com o banco de dados");
            
            return StatusCode(503, new
            {
                status = "offline",
                message = "Não foi possível conectar ao banco de dados",
                error = ex.Message,
                innerException = ex.InnerException?.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
