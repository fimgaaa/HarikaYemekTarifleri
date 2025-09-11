using HarikaYemekTarifleri.Maui.ViewModels;
using HarikaYemekTarifleri.Maui.Models;

namespace HarikaYemekTarifleri.Maui.Pages;

public partial class RecipeEditPage : ContentPage
{
    private readonly RecipeEditViewModel _vm;

    public RecipeEditPage(RecipeEditViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Init();
    }

    private void OnCategoryCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is CategoryDto cat)
        {
            if (e.Value) _vm.SelectedCategoryIds.Add(cat.Id);
            else _vm.SelectedCategoryIds.Remove(cat.Id);
        }
    }
}
