using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Services;
using System.Collections.ObjectModel;

namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class RecipeEditViewModel : BaseViewModel
{
    private readonly IRecipeService _recipes;
    private readonly ICategoryService _cats;
    private readonly INavigation _nav;

    public ObservableCollection<CategoryDto> Categories { get; } = new();

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string content = "";
    [ObservableProperty] private bool isVegetarian;
    [ObservableProperty] private Difficulty difficulty = Difficulty.Easy;
    [ObservableProperty] private TimeSpan prepTime = TimeSpan.FromMinutes(30);
    [ObservableProperty] private DateTime publishDate = DateTime.Today;
    public ObservableCollection<int> SelectedCategoryIds { get; } = new();

    public RecipeEditViewModel(IRecipeService recipes, ICategoryService cats, INavigation nav)
    { _recipes = recipes; _cats = cats; _nav = nav; }

    [RelayCommand]
    public async Task Init()
    {
        var all = await _cats.GetAllAsync();
        Categories.Clear();
        foreach (var c in all) Categories.Add(c);
    }

    [RelayCommand]
    private async Task Save()
    {
        await Guard(async () =>
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content))
                throw new Exception("Başlık ve içerik zorunludur.");

            var dto = new RecipeCreateDto
            {
                Title = Title,
                Content = Content,
                IsVegetarian = IsVegetarian,
                Difficulty = Difficulty,
                PrepTime = PrepTime,
                PublishDate = PublishDate,
                CategoryIds = SelectedCategoryIds.ToList()
            };
            var ok = await _recipes.CreateAsync(dto);
            if (!ok) throw new Exception("Kaydetme başarısız.");
            await _nav.PopAsync();
        });
    }
}
