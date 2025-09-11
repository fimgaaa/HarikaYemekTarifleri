namespace HarikaYemekTarifleri.Api.Models;
public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();
}