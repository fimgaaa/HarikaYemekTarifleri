
using HarikaYemekTarifleri.Maui.Models;

namespace HarikaYemekTarifleri.Maui.Services;

public interface IRecipeService
{
    Task<IEnumerable<RecipeListItem>> SearchAsync(string? q, int? categoryId, bool? vegetarian,
        Difficulty? difficulty, TimeSpan? maxPrep);
    Task<IEnumerable<RecipeListItem>> GetMineAsync();
    Task<IEnumerable<RecipeListItem>> GetByUserAsync(int userId);
    Task<RecipeDetail?> GetAsync(int id);
    Task<int?> CreateAsync(RecipeCreateDto dto);
    Task<bool> UpdateAsync(int id, RecipeUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<string?> UploadRecipePhotoAsync(int recipeId, Stream photo);
}
