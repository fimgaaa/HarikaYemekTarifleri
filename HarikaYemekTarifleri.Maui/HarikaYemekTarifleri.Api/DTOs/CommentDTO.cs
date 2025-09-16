namespace HarikaYemekTarifleri.Api.DTOs;

public class CommentDto
{
    public int Id { get; set; }
    public string CreatedBy { get; set; } = null!;
    public string Text { get; set; } = null!;
}