namespace HarikaYemekTarifleri.Api.Models;
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }          
    public string CreatedBy { get; set; } = null!;   
    public DateTime? UpdatedAt { get; set; }         
    public string? UpdatedBy { get; set; }           
}