namespace HarikaYemekTarifleri.Api.Models;
public enum Difficulty { Easy, Medium, Hard }

public class Recipe : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsVegetarian { get; set; }          // Checkboxfiltres
    public Difficulty Difficulty { get; set; }      // RadioButton 
    public TimeSpan PrepTime { get; set; }          // TimePicker 
    public DateTime PublishDate { get; set; }
    public string? PhotoUrl { get; set; }

    public int UserId { get; set; }             
    public AppUser User { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();
}