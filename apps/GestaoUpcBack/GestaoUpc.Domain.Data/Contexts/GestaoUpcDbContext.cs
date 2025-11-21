using System.Linq;
using Microsoft.EntityFrameworkCore;
using GestaoUpc.Domain.Entities;

namespace GestaoUpc.Domain.Data.Contexts;

public class GestaoUpcDbContext : DbContext
{
    public GestaoUpcDbContext(DbContextOptions<GestaoUpcDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Registra todas as entidades que herdam de BaseEntity
        RegisterAllEntities(modelBuilder);

        // Aplica todas as configurações de mapeamento do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GestaoUpcDbContext).Assembly);
    }

    private void RegisterAllEntities(ModelBuilder modelBuilder)
    {
        var domainAssembly = typeof(User).Assembly;

        var entities = domainAssembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && typeof(BaseEntity).IsAssignableFrom(type));

        foreach (var entity in entities)
        {
            modelBuilder.Entity(entity);
        }
    }
}

