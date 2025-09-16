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
using System.Text;
using System.Text.Json;
file sealed class LoginResponse
{
    public string? token { get; set; }
}

public class AuthService : IAuthService
{
    private readonly ApiClient _api;
    private int? _currentUserId;
    private string? _currentUserName;
    public AuthService(ApiClient api) => _api = api;

    public int? CurrentUserId => _currentUserId;
    public string? CurrentUserName => _currentUserName;

    public async Task<bool> RegisterAsync(string userName, string password, string email)
    {
        var res = await _api.PostAsync("/auth/register", new { UserName = userName, PasswordHash = password, Email=email });
        return res.IsSuccessStatusCode;
    }


    public async Task<bool> LoginAsync(string userName, string password)
    {
        var res = await _api.PostAsync("/auth/login", new { UserName = userName, PasswordHash = password });
        if (!res.IsSuccessStatusCode) return false;

        var dto = await res.Content.ReadFromJsonAsync<LoginResponse>();
        var token = dto?.token;

        if (string.IsNullOrWhiteSpace(token)) return false;

        UpdateCurrentUserFromToken(token);

        _api.SetToken(token);
        return true;
    }

    public Task LogoutAsync()
    {
        UpdateCurrentUserFromToken(null);
        _api.SetToken(null); // ders isterleri 12 (çıkış) – token’ı temizlemek
        return Task.CompletedTask;
    }

    public async Task<bool> ChangePasswordAsync(string oldPwd, string newPwd)
    {
        var res = await _api.PostAsync($"/auth/change-password?oldPassword={oldPwd}&newPassword={newPwd}", new { });
        return res.IsSuccessStatusCode; // ders isterleri 13
    }

    private void UpdateCurrentUserFromToken(string? token)
    {
        _currentUserId = null;
        _currentUserName = null;

        if (string.IsNullOrWhiteSpace(token)) return;

        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2) return;

            var payload = parts[1];
            var bytes = Base64UrlDecode(payload);
            using var doc = JsonDocument.Parse(bytes);
            if (doc.RootElement.TryGetProperty("nameid", out var idProp) &&
                int.TryParse(idProp.GetString(), out var userId))
            {
                _currentUserId = userId;
            }

            if (doc.RootElement.TryGetProperty("unique_name", out var nameProp))
            {
                _currentUserName = nameProp.GetString();
            }
            else if (doc.RootElement.TryGetProperty("name", out var altNameProp))
            {
                _currentUserName = altNameProp.GetString();
            }
        }
        catch
        {
            // token parse başarısızsa kullanıcı bilgilerini sıfırla
            _currentUserId = null;
            _currentUserName = null;
        }
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var builder = new StringBuilder(input.Replace('-', '+').Replace('_', '/'));
        while (builder.Length % 4 != 0)
        {
            builder.Append('=');
        }
        return Convert.FromBase64String(builder.ToString());
    }
}