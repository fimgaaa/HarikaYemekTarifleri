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
    private readonly INavigationService _nav;
    private int? _recipeId;

    public ObservableCollection<CategoryDto> Categories { get; } = new();

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string content = "";
    [ObservableProperty] private bool isVegetarian;
    [ObservableProperty] private Difficulty difficulty = Difficulty.Easy;
    [ObservableProperty] private TimeSpan prepTime = TimeSpan.FromMinutes(30);
    [ObservableProperty] private DateTime publishDate = DateTime.Today;
    public ObservableCollection<int> SelectedCategoryIds { get; } = new();

    public RecipeEditViewModel(IRecipeService recipes, ICategoryService cats, INavigationService nav)
    { _recipes = recipes; _cats = cats; _nav = nav; }

    [RelayCommand]
    public async Task Init()
    {
        var all = await _cats.GetAllAsync();
        Categories.Clear();
        foreach (var c in all) Categories.Add(c);
    }

    public async Task Init(int id)
    {
        await Init();
        _recipeId = id;
        var recipe = await _recipes.GetAsync(id);
        if (recipe is not null)
        {
            Title = recipe.Title;
            Content = recipe.Content;
            IsVegetarian = recipe.IsVegetarian;
            Difficulty = recipe.Difficulty;
            PrepTime = recipe.PrepTime;
            PublishDate = recipe.PublishDate;
            SelectedCategoryIds.Clear();
            foreach (var name in recipe.Categories)
            {
                var cat = Categories.FirstOrDefault(c => c.Name == name);
                if (cat != null) SelectedCategoryIds.Add(cat.Id);
            }
        }
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
            bool ok;
            if (_recipeId.HasValue)
            {
                var updateDto = new RecipeUpdateDto
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    IsVegetarian = dto.IsVegetarian,
                    Difficulty = dto.Difficulty,
                    PrepTime = dto.PrepTime,
                    PublishDate = dto.PublishDate,
                    CategoryIds = dto.CategoryIds
                };
                ok = await _recipes.UpdateAsync(_recipeId.Value, updateDto);
            }
            else
            {
                ok = await _recipes.CreateAsync(dto);
            }
            if (!ok) throw new Exception("Kaydetme başarısız.");
            await _nav.PopAsync();
        });
    }
}
