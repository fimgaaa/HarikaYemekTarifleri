public interface ICommentService
{
    Task<bool> AddAsync(int recipeId, string text);
}

public class CommentService : ICommentService
{
    private readonly ApiClient _api;
    public CommentService(ApiClient api) => _api = api;

    public async Task<bool> AddAsync(int recipeId, string text)
    {
        var res = await _api.PostAsync($"/api/comments?recipeId={recipeId}&text={Uri.EscapeDataString(text)}", new { });
        return res.IsSuccessStatusCode;
    }
}
