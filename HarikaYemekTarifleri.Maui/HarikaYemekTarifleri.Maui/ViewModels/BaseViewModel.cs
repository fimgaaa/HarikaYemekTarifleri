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
}
