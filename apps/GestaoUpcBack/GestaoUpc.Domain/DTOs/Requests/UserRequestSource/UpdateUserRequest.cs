namespace GestaoUpc.Domain.DTOs.Requests.UserRequestSource;

public record UpdateUserRequest
{
    public Guid NavigationId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
}
