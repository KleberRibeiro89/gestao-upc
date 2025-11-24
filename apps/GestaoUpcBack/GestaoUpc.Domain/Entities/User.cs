namespace GestaoUpc.Domain.Entities;

public record User : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Password { get; set; } = null!;
    public bool IsFirstAccess { get; set; } = true;
}
