
namespace HarikaYemekTarifleri.Api.Models;
public class AppUser : BaseEntity
{
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
