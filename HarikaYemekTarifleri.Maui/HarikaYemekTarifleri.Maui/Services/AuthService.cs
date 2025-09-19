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
        _api.SetToken(null); //token’ı temizler. API istemcisinin Bearer token’ı temizlenir; böylece sonraki istekler yetkisiz gider ve korunan endpoint’lere erişemez.
        return Task.CompletedTask;
    }

    public async Task<bool> ChangePasswordAsync(string oldPwd, string newPwd)
    {
        var res = await _api.PostAsync($"/auth/change-password?oldPassword={oldPwd}&newPassword={newPwd}", new { });
        return res.IsSuccessStatusCode; 
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
            var userIdValue = TryGetClaimValue(doc.RootElement,
               "sub",
               "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
               "https://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
               "nameid");

            if (!string.IsNullOrWhiteSpace(userIdValue) && int.TryParse(userIdValue, out var userId))
            {
                _currentUserId = userId;
            }

            var userNameValue = TryGetClaimValue(doc.RootElement,
                "unique_name",
                "name",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                "https://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

            if (!string.IsNullOrWhiteSpace(userNameValue))
            {
                _currentUserName = userNameValue;
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

    private static string? TryGetClaimValue(JsonElement root, params string[] claimNames)
    {
        foreach (var claimName in claimNames)
        {
            if (root.TryGetProperty(claimName, out var claimProp))
            {
                var value = claimProp.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
        }

        return null;
    }
}