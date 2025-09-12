//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HarikaYemekTarifleri.Maui.Services
//{
//    internal class AuthService
//    {
//    }
//}
using System.Net.Http.Json;
file sealed class LoginResponse
{
    public string? token { get; set; }
}

public class AuthService : IAuthService
{
    private readonly ApiClient _api;
    public AuthService(ApiClient api) => _api = api;

    public async Task<bool> RegisterAsync(string userName, string password)
    {
        var res = await _api.PostAsync("/auth/register", new { UserName = userName, PasswordHash = password });
        return res.IsSuccessStatusCode;
    }


    public async Task<bool> LoginAsync(string userName, string password)
    {
        var res = await _api.PostAsync("/auth/login", new { UserName = userName, PasswordHash = password });
        if (!res.IsSuccessStatusCode) return false;

        var dto = await res.Content.ReadFromJsonAsync<LoginResponse>();
        var token = dto?.token;

        if (string.IsNullOrWhiteSpace(token)) return false;

        _api.SetToken(token);
        return true;
    }

    public Task LogoutAsync()
    {
        _api.SetToken(null); // ders isterleri 12 (çıkış) – token’ı temizlemek
        return Task.CompletedTask;
    }

    public async Task<bool> ChangePasswordAsync(string oldPwd, string newPwd)
    {
        var res = await _api.PostAsync($"/auth/change-password?oldPassword={oldPwd}&newPassword={newPwd}", new { });
        return res.IsSuccessStatusCode; // ders isterleri 13
    }
}