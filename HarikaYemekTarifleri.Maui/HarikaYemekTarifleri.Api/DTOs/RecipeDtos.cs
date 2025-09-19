using HarikaYemekTarifleri.Api.Models;

namespace HarikaYemekTarifleri.Api.DTOs;

public class RecipeCreateDto
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsVegetarian { get; set; }
    public Difficulty Difficulty { get; set; }
    public TimeSpan PrepTime { get; set; }
    public DateTime? PublishDate { get; set; }
    public List<int> CategoryIds { get; set; } = new();
    public string? PhotoUrl { get; set; }
}

public class RecipeUpdateDto : RecipeCreateDto { }
