using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Services;
using System.Collections.ObjectModel;
using HarikaYemekTarifleri.Maui.Pages;
using HarikaYemekTarifleri.Maui.Helpers;

namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IUserService _users;
    //public ProfileViewModel(IUserService users) => _users = users;
    private readonly IRecipeService _recipes;
    private readonly INavigationService _navigation;

    public ProfileViewModel(IUserService users, IRecipeService recipes, INavigationService navigation)
    {
        _users = users;
        _recipes = recipes;
        _navigation = navigation;
    }

    public ObservableCollection<RecipeListItem> Recipes { get; } = new();

    [ObservableProperty] private string? userName;
    [ObservableProperty] private string? email;
    [ObservableProperty] private string? photoUrl;

    [RelayCommand]
    public async Task Load()
    {
        await Guard(async () =>
        {
            var profile = await _users.GetProfileAsync();
            if (profile != null)
            {
                UserName = profile.UserName;
                Email = profile.Email;
                PhotoUrl = profile.PhotoUrl;
            }
            var list = await _recipes.GetMineAsync();
            Recipes.Clear();
            foreach (var r in list) Recipes.Add(r);
        });
    }

    [RelayCommand]
    public async Task Save()
    {
        //var profile = new UserProfile { UserName = UserName, Email = Email };
        var profile = new UserProfile { UserName = UserName, Email = Email, PhotoUrl = PhotoUrl };
        await Guard(async () => await _users.UpdateProfileAsync(profile));
    }

    [RelayCommand]
    private async Task AddRecipe()
    {
        var page = ServiceHelper.Get<RecipeEditPage>();
        await _navigation.PushAsync(page);
    }
}