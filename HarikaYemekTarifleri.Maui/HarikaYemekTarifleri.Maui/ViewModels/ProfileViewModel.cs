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

    [RelayCommand]
    private async Task Delete(RecipeListItem item)
    {
        await Guard(async () =>
        {
            var ok = await _recipes.DeleteAsync(item.Id);
            if (ok) Recipes.Remove(item);
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
        //await Guard(async () =>
        //{
        //    var oldPwd = await Application.Current!.MainPage!
        //        .DisplayPromptAsync("Şifre Değiştir", "Mevcut şifre", "Tamam", "İptal", "", -1, keyboard: Keyboard.Text, initialValue: "");
        //    if (oldPwd is null) return;
        //    var newPwd = await Application.Current!.MainPage!
        //        .DisplayPromptAsync("Şifre Değiştir", "Yeni şifre", "Tamam", "İptal", "", -1, keyboard: Keyboard.Text, initialValue: "");
        //    if (newPwd is null) return;
        //    var ok = await _auth.ChangePasswordAsync(oldPwd, newPwd);
        //    await Application.Current!.MainPage!.DisplayAlert(ok ? "Başarılı" : "Başarısız", ok ? "Şifre değiştirildi" : "Şifre değiştirilemedi", "Tamam");
        //});
        var page = ServiceHelper.Get<ChangePasswordPage>();
        await _navigation.PushAsync(page);
    }
}