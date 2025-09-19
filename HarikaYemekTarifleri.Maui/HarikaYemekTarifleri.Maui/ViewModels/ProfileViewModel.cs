using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Services;
using System.Collections.ObjectModel;
using HarikaYemekTarifleri.Maui.Pages;
using HarikaYemekTarifleri.Maui.Helpers;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using System.ComponentModel.DataAnnotations;

namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IUserService _users;
    //public ProfileViewModel(IUserService users) => _users = users;
    private readonly IRecipeService _recipes;
    private readonly INavigationService _navigation;
    private readonly IAuthService _auth;
    private readonly List<RecipeListItem> _allRecipes = new();


    public ProfileViewModel(IUserService users, IRecipeService recipes, INavigationService navigation, IAuthService auth)
    {
        _users = users;
        _recipes = recipes;
        _navigation = navigation;
        _auth = auth;
    }

    public ObservableCollection<RecipeListItem> Recipes { get; } = new();

    [ObservableProperty] private string? userName;
    [ObservableProperty] private string? email;
    [ObservableProperty] private string? searchText;

    partial void OnSearchTextChanged(string? value) => ApplySearchFilter();



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
            }
            var list = await _recipes.GetMineAsync();
            _allRecipes.Clear();
            _allRecipes.AddRange(list);
            ApplySearchFilter();
        });
    }

    [RelayCommand]
    public async Task Save()
    {
        ////var profile = new UserProfile { UserName = UserName, Email = Email };
        //var profile = new UserProfile { UserName = UserName, Email = Email, PhotoUrl = PhotoUrl };
        //await Guard(async () => await _users.UpdateProfileAsync(profile));
        await Guard(async () =>
        {
            if (string.IsNullOrWhiteSpace(UserName))
                throw new Exception("Kullanıcı adı boş olamaz.");

            if (string.IsNullOrWhiteSpace(Email))
                throw new Exception("Email boş olamaz.");

            if (!new EmailAddressAttribute().IsValid(Email))
                throw new Exception("Geçerli bir email adresi giriniz.");

            var profile = new UserProfile { UserName = UserName, Email = Email };
            await _users.UpdateProfileAsync(profile);
        });
    }

    [RelayCommand]
    private async Task AddRecipe()
    {
        var page = ServiceHelper.Get<RecipeEditPage>();
        if (page.BindingContext is RecipeEditViewModel vm)
        {
            await vm.PrepareForCreateAsync();
        }
        await _navigation.PushAsync(page);
    }

    [RelayCommand]
    public async Task Delete(RecipeListItem item)
    {
        await Guard(async () =>
        {
            var confirmed = await (Application.Current?.MainPage?
            .DisplayAlert("Tarifi Sil", $"{item.Title} silinsin mi?", "Evet", "Vazgeç")
            ?? Task.FromResult(false));

                    if (!confirmed)
                        return;

            var ok = await _recipes.DeleteAsync(item.Id);
            if (ok)
            {
                var existing = _allRecipes.FirstOrDefault(r => r.Id == item.Id);
                if (existing != null)
                    _allRecipes.Remove(existing);
                ApplySearchFilter();
            }
        });
    }

    [RelayCommand]
    private async Task OpenRecipe(RecipeListItem item)
    {
        var page = ServiceHelper.Get<RecipeDetailPage>();
        if (page.BindingContext is RecipeDetailViewModel vm)
            await vm.Load(item.Id);
        await _navigation.PushAsync(page);
    }

    [RelayCommand]
    private async Task ChangePassword()
    {
        var page = ServiceHelper.Get<ChangePasswordPage>();
        await _navigation.PushAsync(page);
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

    private void ApplySearchFilter()
    {
        var query = SearchText?.Trim();
        IEnumerable<RecipeListItem> filtered = _allRecipes;

        if (!string.IsNullOrWhiteSpace(query))
        {
            filtered = _allRecipes.Where(r =>
                !string.IsNullOrWhiteSpace(r.Title) &&
                r.Title!.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        }

        Recipes.Clear();
        foreach (var recipe in filtered)
            Recipes.Add(recipe);
    }
}