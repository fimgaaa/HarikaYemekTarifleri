using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using HarikaYemekTarifleri.Maui.Services;
using HarikaYemekTarifleri.Maui.Pages;
using HarikaYemekTarifleri.Maui.Helpers; // <-- önemli

namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _auth;

    [ObservableProperty] private string userName = "";
    [ObservableProperty] private string password = "";

    public LoginViewModel(IAuthService auth) => _auth = auth;

    [RelayCommand]
    private async Task Login()
    {
        await Guard(async () =>
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
                throw new Exception("Kullanıcı adı/şifre zorunlu.");

            var ok = await _auth.LoginAsync(UserName, Password);
            if (!ok) throw new Exception("Giriş başarısız.");

            // DI'dan sayfayı al ve Navigation'a push et
            var page = ServiceHelper.Get<RecipesPage>();
            await Application.Current!.MainPage!.Navigation.PushAsync(page);
        });
    }

    [RelayCommand]
    private async Task NavigateToRegister()
    {
        var page = ServiceHelper.Get<RegisterPage>();
        await Application.Current!.MainPage!.Navigation.PushAsync(page);
    }
}
