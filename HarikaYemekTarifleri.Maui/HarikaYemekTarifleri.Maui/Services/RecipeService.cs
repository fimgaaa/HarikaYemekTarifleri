using HarikaYemekTarifleri.Maui.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace HarikaYemekTarifleri.Maui.Services;

public class RecipeService : IRecipeService
{
    private readonly ApiClient _api;
    public RecipeService(ApiClient api) => _api = api;

    public async Task<IEnumerable<RecipeListItem>> SearchAsync(string? q, int? categoryId, bool? vegetarian,
        Difficulty? difficulty, TimeSpan? maxPrep)
    {
        var qs = new List<string>();
        if (!string.IsNullOrWhiteSpace(q)) qs.Add($"q={Uri.EscapeDataString(q)}");
        if (categoryId.HasValue) qs.Add($"categoryId={categoryId.Value}");
        if (vegetarian.HasValue) qs.Add($"vegetarian={vegetarian.Value.ToString().ToLower()}");
        if (difficulty.HasValue) qs.Add($"difficulty={(int)difficulty.Value}");
        //if (fromDate.HasValue) qs.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
        if (maxPrep.HasValue) qs.Add($"maxPrep={maxPrep.Value}");
        var url = "/api/recipes" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

        var res = await _api.GetAsync(url);
        res.EnsureSuccessStatusCode();
        //return await res.Content.ReadFromJsonAsync<IEnumerable<RecipeListItem>>() ?? [];
        var list = await res.Content.ReadFromJsonAsync<IEnumerable<RecipeListItem>>() ?? [];
        return list;
    }
    public async Task<IEnumerable<RecipeListItem>> GetMineAsync()
    {
        var res = await _api.GetAsync("/api/recipes/mine");
        res.EnsureSuccessStatusCode();
        //return await res.Content.ReadFromJsonAsync<IEnumerable<RecipeListItem>>() ?? [];
        var list = await res.Content.ReadFromJsonAsync<IEnumerable<RecipeListItem>>() ?? [];
        return list;
    }
    public async Task<IEnumerable<RecipeListItem>> GetByUserAsync(int userId)
    {
        var res = await _api.GetAsync($"/api/recipes/user/{userId}");
        res.EnsureSuccessStatusCode();
        var list = await res.Content.ReadFromJsonAsync<IEnumerable<RecipeListItem>>() ?? [];
        return list;
    }
    public async Task<RecipeDetail?> GetAsync(int id)
    {
        var res = await _api.GetAsync($"/api/recipes/{id}");
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<RecipeDetail>();
    }

    public async Task<int?> CreateAsync(RecipeCreateDto dto)
    {
        var res = await _api.PostAsync("/api/recipes", dto);
        if (!res.IsSuccessStatusCode) return null;
        var created = await res.Content.ReadFromJsonAsync<RecipeDetail>();
        return created?.Id;
    }

    public async Task<bool> UpdateAsync(int id, RecipeUpdateDto dto)
    {
        var res = await _api.PutAsync($"/api/recipes/{id}", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var res = await _api.DeleteAsync($"/api/recipes/{id}");
        return res.IsSuccessStatusCode;
    }

    public async Task<string?> UploadRecipePhotoAsync(int recipeId, Stream photo)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(photo), "photo", "photo.jpg");
        var res = await _api.PostAsync($"/api/recipes/{recipeId}/photo", content);
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        if (json.TryGetProperty("url", out var url)) return url.GetString();
        return null;
    }
}
