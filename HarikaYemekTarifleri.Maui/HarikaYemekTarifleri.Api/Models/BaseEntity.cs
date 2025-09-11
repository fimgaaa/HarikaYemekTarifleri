namespace HarikaYemekTarifleri.Api.Models;
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }          // #10
    public string CreatedBy { get; set; } = null!;   // #10
    public DateTime? UpdatedAt { get; set; }         // #10
    public string? UpdatedBy { get; set; }           // #10
}