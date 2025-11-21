namespace GestaoUpc.Domain.DTOs.Responses;

public abstract record BaseEntityResponse
{
    public Guid NavigationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool Active { get; set; }
}