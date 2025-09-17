using HarikaYemekTarifleri.Maui.Models;

namespace HarikaYemekTarifleri.Maui.Services;

public interface IUserService
{
    Task<UserProfile?> GetProfileAsync();
    Task<bool> UpdateProfileAsync(UserProfile profile);
}