using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Services;
using Microsoft.Maui.Controls;

namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class ChangePasswordViewModel : BaseViewModel
{
    private readonly IAuthService _auth;
    private readonly INavigationService _navigation;

    [ObservableProperty]
    private string? oldPassword;

    [ObservableProperty]
    private string? newPassword;

    public ChangePasswordViewModel(IAuthService auth, INavigationService navigation)
    {
        _auth = auth;
        _navigation = navigation;
    }

    [RelayCommand]
    private async Task Save()
    {
        await Guard(async () =>
        {
            if (string.IsNullOrWhiteSpace(OldPassword))
            {
                throw new InvalidOperationException("Lütfen mevcut şifrenizi girin.");
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                throw new InvalidOperationException("Lütfen yeni şifrenizi girin.");
            }

            if (OldPassword == NewPassword)
            {
                throw new InvalidOperationException("Yeni şifre mevcut şifre ile aynı olamaz.");
            }

            var ok = await _auth.ChangePasswordAsync(OldPassword!, NewPassword!);
            await Application.Current!.MainPage!.DisplayAlert(ok ? "Başarılı" : "Başarısız",
                ok ? "Şifre değiştirildi" : "Şifre değiştirilemedi", "Tamam");
            if (!ok)
            {
                Error = "Şifre değiştirilemedi";
            }
            if (ok)
            {
                await _navigation.PopAsync();
            }
        });
    }
}