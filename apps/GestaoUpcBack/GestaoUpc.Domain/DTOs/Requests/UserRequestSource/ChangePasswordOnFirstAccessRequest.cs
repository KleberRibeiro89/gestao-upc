namespace GestaoUpc.Domain.DTOs.Requests.UserRequestSource;

public record ChangePasswordOnFirstAccessRequest
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = null!;
}

