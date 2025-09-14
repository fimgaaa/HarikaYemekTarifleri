using HarikaYemekTarifleri.Maui.Models;
using System.Net.Http.Json;
using System.Net.Http;
using System.IO;

namespace HarikaYemekTarifleri.Maui.Services;

public class UserService : IUserService
{
    private readonly ApiClient _api;
    public UserService(ApiClient api) => _api = api;

    public async Task<UserProfile?> GetProfileAsync()
    {
        var res = await _api.GetAsync("/api/users/me");
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<UserProfile>();
    }

    public async Task<bool> UpdateProfileAsync(UserProfile profile)
    {
        var res = await _api.PutAsync("/api/users/me", profile);
        return res.IsSuccessStatusCode;
    }

    public async Task<string?> UploadPhotoAsync(Stream photo)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(photo), "photo", "photo.jpg");
        var res = await _api.PostAsync("/api/users/me/photo", content);
        if (!res.IsSuccessStatusCode) return null;
        var dto = await res.Content.ReadFromJsonAsync<UploadPhotoResponse>();
        return dto?.url;
    }

    private sealed class UploadPhotoResponse
    {
        public string url { get; set; } = null!;
    }
}