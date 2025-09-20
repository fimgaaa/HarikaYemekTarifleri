using CommunityToolkit.Mvvm.ComponentModel;

namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? error;

    protected async Task Guard(Func<Task> action)
    {
        try { IsBusy = true; Error = null; await action(); }
        catch (Exception ex) { Error = ex.Message; }
        finally { IsBusy = false; }
        await Task.CompletedTask;
    }
    protected static void EnsurePasswordComplexity(string password)
    {
        if (password.Length < 8)
        {
            throw new InvalidOperationException("Şifre en az 8 karakter olmalıdır.");
        }

        if (!password.Any(char.IsLower) || !password.Any(char.IsUpper) || !password.Any(char.IsDigit))
        {
            throw new InvalidOperationException("Şifre en az bir küçük harf, bir büyük harf ve bir rakam içermelidir.");
        }
    }
}
