using HarikaYemekTarifleri.Maui.Models;
using System.IO;

namespace HarikaYemekTarifleri.Maui.Services;

public interface IUserService
{
    Task<UserProfile?> GetProfileAsync();
    Task<bool> UpdateProfileAsync(UserProfile profile);
    Task<string?> UploadPhotoAsync(Stream photo);
}