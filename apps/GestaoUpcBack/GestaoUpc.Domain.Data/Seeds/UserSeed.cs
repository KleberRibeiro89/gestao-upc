using GestaoUpc.Domain.Data.Contexts;
using GestaoUpc.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoUpc.Domain.Data.Seeds;

public static class UserSeed
{
    private static readonly Guid AdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private const string AdminEmail = "admin@gestaoupc.com";
    private const string AdminName = "Administrador";
    private const string AdminPasswordHash = "$2a$11$h1iFdeMEPzipmBUI./UaXeeTf3ehi9oariiV/.MkISMbFNChFuw8m";

    public static async Task SeedAsync(GestaoUpcDbContext context)
    {
        // Verifica se já existe um usuário admin
        var adminExists = await context.Set<User>()
            .AnyAsync(u => u.Email == AdminEmail && u.Active);

        if (adminExists)
        {
            return;
        }

        var adminUser = new User
        {
            NavigationId = AdminUserId,
            Name = AdminName,
            Email = AdminEmail,
            Password = AdminPasswordHash,
            IsFirstAccess = false,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = AdminUserId
        };

        context.Set<User>().Add(adminUser);
        await context.SaveChangesAsync();
    }
}

