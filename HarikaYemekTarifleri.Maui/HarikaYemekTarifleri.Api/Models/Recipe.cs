namespace HarikaYemekTarifleri.Api.Models;
public enum Difficulty { Easy, Medium, Hard }

public class Recipe : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsVegetarian { get; set; }          // Checkbox filtresi (#17)
    public Difficulty Difficulty { get; set; }      // RadioButton (#17)
    public TimeSpan PrepTime { get; set; }          // TimePicker (#16)

    public string? PhotoUrl { get; set; }

    public int UserId { get; set; }                 // Sahibi
    public AppUser User { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();
}