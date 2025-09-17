using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using System.IO;


namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class RecipeEditViewModel : BaseViewModel
{
    private readonly IRecipeService _recipes;
    private readonly ICategoryService _cats;
    private readonly INavigationService _nav;
    private int? _recipeId;
    private FileResult? _photoFile;

    public ObservableCollection<CategoryDto> Categories { get; } = new();

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string content = "";
    [ObservableProperty] private bool isVegetarian;
    [ObservableProperty] private Difficulty difficulty = Difficulty.Easy;
    [ObservableProperty] private TimeSpan prepTime = TimeSpan.FromMinutes(30);
    [ObservableProperty] private DateTime? createdAt;

    public bool HasCreatedAt => CreatedAt.HasValue;

    partial void OnCreatedAtChanged(DateTime? value)
    {
        OnPropertyChanged(nameof(HasCreatedAt));
    }

    [ObservableProperty] private string? photoUrl;
    public ObservableCollection<int> SelectedCategoryIds { get; } = new();

    public List<Option<Difficulty>> DifficultyOptions { get; } =
[
    new("Kolay", Difficulty.Easy),
    new("Orta", Difficulty.Medium),
    new("Zor", Difficulty.Hard)
];

    [ObservableProperty] private Option<Difficulty>? selectedDifficulty;

    partial void OnSelectedDifficultyChanged(Option<Difficulty>? value)
    {
        Difficulty = value?.Value ?? Difficulty.Easy;
    }

    public RecipeEditViewModel(IRecipeService recipes, ICategoryService cats, INavigationService nav)
    { _recipes = recipes; _cats = cats; _nav = nav; }

    [RelayCommand]
    public async Task Init()
    {
        var all = await _cats.GetAllAsync();
        Categories.Clear();
        foreach (var c in all) Categories.Add(c);
        SelectedDifficulty = DifficultyOptions.FirstOrDefault(o => o.Value == Difficulty);
        CreatedAt = null;
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
            SelectedDifficulty = DifficultyOptions.FirstOrDefault(o => o.Value == recipe.Difficulty);
            PrepTime = recipe.PrepTime;
            CreatedAt = recipe.CreatedAt;

            PhotoUrl = recipe.PhotoUrl;
            SelectedCategoryIds.Clear();
            foreach (var name in recipe.Categories)
            {
                var cat = Categories.FirstOrDefault(c => c.Name == name);
                if (cat != null) SelectedCategoryIds.Add(cat.Id);
            }
        }
    }

    [RelayCommand]
    private async Task SelectPhoto()
    {
        await Guard(async () =>
        {
            FileResult? file = null;
            try
            {
                file = await MediaPicker.Default.PickPhotoAsync();
            }
            catch (FeatureNotSupportedException)
            {
                file = await FilePicker.Default.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Fotoğraf Seç"
                });
            }

            if (file != null)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                    throw new Exception("Sadece JPG veya PNG dosyaları seçilebilir.");
                using var stream = await file.OpenReadAsync();
                if (stream.Length > 5 * 1024 * 1024)
                    throw new Exception("Dosya boyutu 5MB'dan küçük olmalı.");
                _photoFile = file;
                PhotoUrl = file.FullPath;
            }
        });
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
                CategoryIds = SelectedCategoryIds.ToList(),
                PhotoUrl = PhotoUrl
            };
            
            if (_recipeId.HasValue)
            {
                var updateDto = new RecipeUpdateDto
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    IsVegetarian = dto.IsVegetarian,
                    Difficulty = dto.Difficulty,
                    PrepTime = dto.PrepTime,
                    CategoryIds = dto.CategoryIds,
                    PhotoUrl = dto.PhotoUrl
                };
                try
                {
                    var ok = await _recipes.UpdateAsync(_recipeId.Value, updateDto);
                    if (!ok) throw new Exception("Kaydetme başarısız.");
                }
                catch (UnauthorizedAccessException ex)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Yetkisiz", ex.Message, "Tamam");
                    throw;
                }

                if (_photoFile != null)
                {
                    using var stream = await _photoFile.OpenReadAsync();
                    var url = await _recipes.UploadRecipePhotoAsync(_recipeId.Value, stream);
                    if (url != null) PhotoUrl = url;
                }
            }
            else
            {
                var newId = await _recipes.CreateAsync(dto);
                if (newId is null) throw new Exception("Kaydetme başarısız.");
                _recipeId = newId;
                if (_photoFile != null)
                {
                    using var stream = await _photoFile.OpenReadAsync();
                    var url = await _recipes.UploadRecipePhotoAsync(newId.Value, stream);
                    if (url != null) PhotoUrl = url;
                }
            }

            await _nav.PopAsync();
        });
    }
}
