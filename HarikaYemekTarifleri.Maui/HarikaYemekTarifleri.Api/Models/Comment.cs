namespace HarikaYemekTarifleri.Api.Models;
public class Comment : BaseEntity
{
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public string Text { get; set; } = null!;
}
