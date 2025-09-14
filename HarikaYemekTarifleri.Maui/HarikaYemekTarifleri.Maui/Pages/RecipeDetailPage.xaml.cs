using HarikaYemekTarifleri.Maui.ViewModels;

namespace HarikaYemekTarifleri.Maui.Pages;

public partial class RecipeDetailPage : ContentPage
{
    private readonly RecipeDetailViewModel _vm;
    public RecipeDetailPage(RecipeDetailViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }
}