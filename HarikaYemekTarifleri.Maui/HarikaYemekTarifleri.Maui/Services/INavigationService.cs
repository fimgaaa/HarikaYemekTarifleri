using Microsoft.Maui.Controls;

namespace HarikaYemekTarifleri.Maui.Services;

public interface INavigationService
{
    Task PushAsync(Page page);
    Task PopAsync();
}