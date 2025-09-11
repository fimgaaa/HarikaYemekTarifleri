using HarikaYemekTarifleri.Maui.Models;

namespace HarikaYemekTarifleri.Maui.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
}
