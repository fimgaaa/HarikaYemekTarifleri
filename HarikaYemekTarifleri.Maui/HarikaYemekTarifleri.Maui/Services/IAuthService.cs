
using System.Net.Http.Json;

public interface IAuthService
{
    Task<bool> LoginAsync(string userName, string password);
    Task<bool> RegisterAsync(string userName, string password, string email);
    Task LogoutAsync();
    Task<bool> ChangePasswordAsync(string oldPwd, string newPwd);
    int? CurrentUserId { get; }
    string? CurrentUserName { get; }
}
