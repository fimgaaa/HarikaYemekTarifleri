using HarikaYemekTarifleri.Maui.Models;
using System.Net.Http.Json;

namespace HarikaYemekTarifleri.Maui.Services;

public class CategoryService : ICategoryService
{
    private readonly ApiClient _api;
    public CategoryService(ApiClient api) => _api = api;

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var res = await _api.GetAsync("/api/categories");
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new();
    }
}
