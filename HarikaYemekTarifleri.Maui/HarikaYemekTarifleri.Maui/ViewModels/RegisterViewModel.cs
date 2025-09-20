using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Services;
using Microsoft.Maui.Controls;

namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _auth;

    [ObservableProperty] private string userName = "";
    [ObservableProperty] private string email = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string confirmPassword = "";

    public RegisterViewModel(IAuthService auth) => _auth = auth;

    [RelayCommand]
    private async Task Register()
    {
        await Guard(async () =>
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Email)
                || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
                throw new Exception("Tüm alanlar zorunludur.");

            if (Password != ConfirmPassword)
                throw new Exception("Şifreler uyuşmuyor.");

            EnsurePasswordComplexity(Password);

            var ok = await _auth.RegisterAsync(UserName, Password, Email);
            if (!ok) throw new Exception("Kayıt başarısız.");

            await Application.Current!.MainPage!.Navigation.PopAsync();
        });
    }
}