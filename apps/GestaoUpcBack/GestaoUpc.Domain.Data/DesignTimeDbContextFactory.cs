using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GestaoUpc.Domain.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Contexts.GestaoUpcDbContext>
{
    public Contexts.GestaoUpcDbContext CreateDbContext(string[] args)
    {
        // Tenta encontrar o appsettings.json no diretório da API
        var currentDirectory = Directory.GetCurrentDirectory();
        var basePath = Path.Combine(currentDirectory, "..", "GestaoUpc.Api");
        
        // Se não encontrar, tenta subir mais um nível (caso esteja em um subdiretório)
        if (!Directory.Exists(basePath))
        {
            basePath = Path.Combine(currentDirectory, "..", "..", "GestaoUpc.Api");
        }
        
        // Se ainda não encontrar, tenta no diretório atual
        if (!Directory.Exists(basePath))
        {
            basePath = currentDirectory;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration["DATABASE_PUBLIC_URL"] 
            ?? configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DATABASE_PUBLIC_URL' not found or is empty. " +
                "Please configure DATABASE_PUBLIC_URL in appsettings.json or as an environment variable.");
        }

        // Converte URI para connection string se necessário
        string normalizedConnectionString;
        if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
            connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var uri = new Uri(connectionString);
                var csBuilder = new Npgsql.NpgsqlConnectionStringBuilder
                {
                    Host = uri.Host,
                    Port = uri.Port > 0 ? uri.Port : 5432,
                    Database = uri.AbsolutePath.TrimStart('/'),
                    Username = uri.UserInfo.Split(':')[0],
                    Password = uri.UserInfo.Contains(':') ? uri.UserInfo.Split(':')[1] : string.Empty
                };
                normalizedConnectionString = csBuilder.ConnectionString;
            }
            catch
            {
                // Se falhar na conversão, usa a connection string original
                normalizedConnectionString = connectionString;
            }
        }
        else
        {
            normalizedConnectionString = connectionString;
        }

        var optionsBuilder = new DbContextOptionsBuilder<Contexts.GestaoUpcDbContext>();
        optionsBuilder.UseNpgsql(normalizedConnectionString);

        return new Contexts.GestaoUpcDbContext(optionsBuilder.Options);
    }
}

