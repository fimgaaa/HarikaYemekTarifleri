using Microsoft.Maui.Controls;

namespace HarikaYemekTarifleri.Maui.Services;

public class NavigationService : INavigationService
{
    public Task PushAsync(Page page) => Application.Current!.MainPage!.Navigation.PushAsync(page);

    public Task PopAsync() => Application.Current!.MainPage!.Navigation.PopAsync();
}