using HarikaYemekTarifleri.Maui.ViewModels;

namespace HarikaYemekTarifleri.Maui.Pages;

public partial class RecipesPage : ContentPage
{
    private readonly RecipesViewModel _vm;
    public RecipesPage(RecipesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Load();
    }
}
