using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Helpers;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Services;
using System.Collections.ObjectModel;
using HarikaYemekTarifleri.Maui.Pages;
using Microsoft.Maui.Controls;

namespace HarikaYemekTarifleri.Maui.ViewModels;

public record Option<T>(string Name, T Value);

public partial class RecipesViewModel : BaseViewModel
{
    private readonly IRecipeService _recipes;
    private readonly ICategoryService _cats;
    private readonly INavigationService _navigation;
    private readonly IAuthService _auth;

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
    //[ObservableProperty] private DateTime? fromDate;
    [ObservableProperty] private TimeSpan? maxPrep;

    //public RecipesViewModel(IRecipeService recipes, ICategoryService cats, INavigationService navigation)
    public RecipesViewModel(IRecipeService recipes, ICategoryService cats, INavigationService navigation, IAuthService auth)
    {
        //_recipes = recipes; _cats = cats; _navigation = navigation;
        _recipes = recipes; _cats = cats; _navigation = navigation; _auth = auth;
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
                Categories.Insert(0, new CategoryDto { Id = 0, Name = "Tümü" });
                SelectedCategory = Categories[0];
            }
            var list = await _recipes.SearchAsync(Q, SelectedCategory?.Id == 0 ? null : SelectedCategory?.Id, Vegetarian,
                SelectedDifficulty?.Value, MaxPrep);
            Items.Clear();
            foreach (var x in list) Items.Add(x);
        });
    }

    //[RelayCommand]
    //private async Task NewRecipe()
    //{
    //    var page = ServiceHelper.Get<Pages.RecipeEditPage>();
    //    await _navigation.PushAsync(page);
    //}

    [RelayCommand]
    private async Task Profile()
    {
        var page = ServiceHelper.Get<ProfilePage>();
        await _navigation.PushAsync(page);
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

    [RelayCommand]
    private async Task Logout()
    {
        await Guard(async () =>
        {
            await _auth.LogoutAsync();
            await Application.Current!.MainPage!.DisplayAlert("Başarılı", "Çıkış yapıldı", "Tamam");
            await Application.Current!.MainPage!.Navigation.PopToRootAsync();
        });
    }

    //[RelayCommand]
    //private async Task ChangePassword()
    //{
    //    await Guard(async () =>
    //    {
    //        var oldPwd = await Application.Current!.MainPage!
    //            .DisplayPromptAsync("Şifre Değiştir", "Mevcut şifre", "Tamam", "İptal", "", -1, keyboard: Keyboard.Text, initialValue: "");
    //        if (oldPwd is null) return;
    //        var newPwd = await Application.Current!.MainPage!
    // .DisplayPromptAsync("Şifre Değiştir", "Yeni şifre", "Tamam", "İptal", "", -1, keyboard: Keyboard.Text, initialValue: "");
    //        if (newPwd is null) return;
    //        var ok = await _auth.ChangePasswordAsync(oldPwd, newPwd);
    //        await Application.Current!.MainPage!.DisplayAlert(ok ? "Başarılı" : "Başarısız", ok ? "Şifre değiştirildi" : "Şifre değiştirilemedi", "Tamam");
    //    });
    //}
}
