namespace GestaoUpc.Domain.DTOs.Responses.UserResponseSource;

public record UserResponse : BaseEntityResponse
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsFirstAccess { get; set; }
}
