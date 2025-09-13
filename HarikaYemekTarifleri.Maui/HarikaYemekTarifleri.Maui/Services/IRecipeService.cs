//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HarikaYemekTarifleri.Maui.Services
//{
//    internal class IRecipeService
//    {
//    }
//}

using HarikaYemekTarifleri.Maui.Models;

namespace HarikaYemekTarifleri.Maui.Services;

public interface IRecipeService
{
    Task<IEnumerable<RecipeListItem>> SearchAsync(string? q, int? categoryId, bool? vegetarian,
        Difficulty? difficulty, TimeSpan? maxPrep);
    Task<IEnumerable<RecipeListItem>> GetMineAsync();
    Task<RecipeDetail?> GetAsync(int id);
    Task<bool> CreateAsync(RecipeCreateDto dto);
    Task<bool> UpdateAsync(int id, RecipeUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
