using HarikaYemekTarifleri.Maui.Models;
using System.Net.Http.Json;
using System.Net.Http;


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

}