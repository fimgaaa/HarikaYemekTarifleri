
using HarikaYemekTarifleri.Maui.Models;
using System.Net.Http.Json;

public interface ICommentService
{
    Task<CommentDto?> AddAsync(int recipeId, string text);
}

public class CommentService : ICommentService
{
    private readonly ApiClient _api;
    public CommentService(ApiClient api) => _api = api;

    public async Task<CommentDto?> AddAsync(int recipeId, string text)
    {
        var res = await _api.PostAsync($"/api/comments?recipeId={recipeId}&text={Uri.EscapeDataString(text)}", new { });
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<CommentDto>();
    }
}