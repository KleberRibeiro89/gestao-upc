using GestaoUpc.Domain.AppService.Services;
using GestaoUpc.Domain.AppService.Validators;
using GestaoUpc.Domain.Data.Contexts;
using GestaoUpc.Domain.Data.Repositories;
using GestaoUpc.Domain.Mappings;
using GestaoUpc.Domain.Repositories;
using GestaoUpc.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Npgsql;

namespace GestaoUpc.Infra.Ioc;

public static class DependencyInjection
{
    private const string ConnectionString = "DefaultConnection";

    public static IServiceCollection AddApiInversionOfControl(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddContext(configuration);
        services.AddRepositories();
        services.AddServices();
        services.AddAutoMapper();
        services.AddFluentValidation();

        services.AddMemoryCache();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserService, UserService>();

        return services; 
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>))
                .AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DATABASE_PUBLIC_URL"] 
            ?? configuration.GetConnectionString(ConnectionString);

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
                var csBuilder = new NpgsqlConnectionStringBuilder
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

        services
            .AddDbContext<GestaoUpcDbContext>(options => options.UseNpgsql(normalizedConnectionString));

        return services;
    }

    private static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DomainToResponseMapping).Assembly);
        return services;
    }

    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        return services;
    }
}
