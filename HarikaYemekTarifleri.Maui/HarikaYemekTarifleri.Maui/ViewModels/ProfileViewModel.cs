using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Services;

namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IUserService _users;
    public ProfileViewModel(IUserService users) => _users = users;

    [ObservableProperty] private string? userName;
    [ObservableProperty] private string? email;

    [RelayCommand]
    public async Task Load()
    {
        await Guard(async () =>
        {
            var profile = await _users.GetProfileAsync();
            if (profile != null)
            {
                UserName = profile.UserName;
                Email = profile.Email;
            }
        });
    }

    [RelayCommand]
    public async Task Save()
    {
        var profile = new UserProfile { UserName = UserName, Email = Email };
        await Guard(async () => await _users.UpdateProfileAsync(profile));
    }
}