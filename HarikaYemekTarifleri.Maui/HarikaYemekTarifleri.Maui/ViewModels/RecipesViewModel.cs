using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Helpers;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Services;
using System.Collections.ObjectModel;

namespace HarikaYemekTarifleri.Maui.ViewModels;

public record Option<T>(string Name, T Value);

public partial class RecipesViewModel : BaseViewModel
{
    private readonly IRecipeService _recipes;
    private readonly ICategoryService _cats;
    private readonly INavigation _nav;

    public ObservableCollection<RecipeListItem> Items { get; } = new();
    public ObservableCollection<CategoryDto> Categories { get; } = new();

    [ObservableProperty] private string? q;
    [ObservableProperty] private CategoryDto? selectedCategory;
    [ObservableProperty] private bool? vegetarian;
    public List<Option<Difficulty>> DifficultyOptions { get; } =
    [
        new("Kolay", Difficulty.Easy),
        new("Orta", Difficulty.Medium),
        new("Zor", Difficulty.Hard)
    ];
    [ObservableProperty] private Option<Difficulty>? selectedDifficulty;
    [ObservableProperty] private DateTime? fromDate;
    [ObservableProperty] private TimeSpan? maxPrep;

    public RecipesViewModel(IRecipeService recipes, ICategoryService cats, INavigation nav)
    {
        _recipes = recipes; _cats = cats; _nav = nav;
    }

    [RelayCommand]
    public async Task Load()
    {
        await Guard(async () =>
        {
            if (Categories.Count == 0)
            {
                var all = await _cats.GetAllAsync();
                foreach (var c in all) Categories.Add(c);
            }
            var list = await _recipes.SearchAsync(Q, SelectedCategory?.Id, Vegetarian,
                SelectedDifficulty?.Value, FromDate, MaxPrep);
            Items.Clear();
            foreach (var x in list) Items.Add(x);
        });
    }

    [RelayCommand]
    private async Task NewRecipe()
    {
        var page = ServiceHelper.Get<Pages.RecipeEditPage>();
        await Application.Current!.MainPage!.Navigation.PushAsync(page);
    }

    [RelayCommand]
    private async Task Delete(RecipeListItem item)
    {
        await Guard(async () =>
        {
            var ok = await _recipes.DeleteAsync(item.Id);
            if (ok) Items.Remove(item);
        });
    }
}
