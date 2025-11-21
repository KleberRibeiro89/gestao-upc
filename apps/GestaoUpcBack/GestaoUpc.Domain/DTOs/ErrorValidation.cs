namespace GestaoUpc.Domain.DTOs;

public record ErrorValidation
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}