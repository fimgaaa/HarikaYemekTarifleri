namespace HarikaYemekTarifleri.Maui.Models;

public enum Difficulty { Easy = 0, Medium = 1, Hard = 2 }

public record RecipeListItem(
    int Id, string Title, bool IsVegetarian, Difficulty Difficulty,
        //TimeSpan PrepTime, DateTime PublishDate, IEnumerable<string> Categories, int CommentsCount);
    TimeSpan PrepTime, DateTime CreatedAt, IEnumerable<string> Categories,
    int CommentsCount, string UserName, string? PhotoUrl);

public class RecipeDetail
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public bool IsVegetarian { get; set; }
    public Difficulty Difficulty { get; set; }
    public TimeSpan PrepTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Categories { get; set; } = new();
    public int UserId { get; set; }
    public string? CreatedBy { get; set; }
    //public List<string> Comments { get; set; } = new();
    public string? PhotoUrl { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}

public class RecipeCreateDto
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsVegetarian { get; set; }
    public Difficulty Difficulty { get; set; }
    public TimeSpan PrepTime { get; set; }
    public List<int> CategoryIds { get; set; } = new();
    public string? PhotoUrl { get; set; }
}
public class RecipeUpdateDto : RecipeCreateDto { }

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
