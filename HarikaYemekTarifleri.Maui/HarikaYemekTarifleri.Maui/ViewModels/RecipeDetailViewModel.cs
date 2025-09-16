using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Helpers;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Pages;
using HarikaYemekTarifleri.Maui.Services;
using System.Collections.ObjectModel;
using System.Linq;


namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly IRecipeService _recipes;
    private readonly ICommentService _comments;
    private readonly INavigationService _navigation;
    private readonly IAuthService _auth;

    public ObservableCollection<CommentDto> Comments { get; } = new();
    public ObservableCollection<RecipeListItem> AuthorRecipes { get; } = new();

    [ObservableProperty] private RecipeDetail? recipe;
    [ObservableProperty] private string? newComment;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    private bool isOwner;
    private int _recipeId;

    public RecipeDetailViewModel(IRecipeService recipes, ICommentService comments, INavigationService navigation, IAuthService auth)
    {
        _recipes = recipes;
        _comments = comments;
        _navigation = navigation;
        _auth = auth;
    }

    public async Task Load(int id)
    {
        _recipeId = id;
        IsOwner = false;
        await Guard(async () =>
        {
            var detail = await _recipes.GetAsync(id);
            Recipe = detail;
            Comments.Clear();
            AuthorRecipes.Clear();
            if (detail is null) return;

            if (detail.Comments is not null)
            {
                foreach (var c in detail.Comments)
                    Comments.Add(c);
            }

            var authorRecipes = await _recipes.GetByUserAsync(detail.UserId);
            foreach (var item in authorRecipes.Where(r => r.Id != detail.Id))
                AuthorRecipes.Add(item);
        });
    }

    [RelayCommand]
    private async Task AddComment()
    {
        if (string.IsNullOrWhiteSpace(NewComment)) return;
        await Guard(async () =>
        {
            var dto = await _comments.AddAsync(_recipeId, NewComment!);
            if (dto is not null)
            {
                Comments.Add(dto);
                NewComment = string.Empty;
            }
        });
    }

    partial void OnRecipeChanged(RecipeDetail? value)
    {
        if (value is null)
        {
            IsOwner = false;
            return;
        }

        var owns = false;
        if (_auth.CurrentUserId.HasValue)
        {
            owns = value.UserId == _auth.CurrentUserId.Value;
        }

        IsOwner = owns;
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private async Task Edit()
    {
        var page = ServiceHelper.Get<RecipeEditPage>();
        if (page.BindingContext is RecipeEditViewModel vm)
            await vm.Init(_recipeId);
        await _navigation.PushAsync(page);
    }

    private bool CanEdit() => IsOwner;

    [RelayCommand]
    private async Task OpenRecipe(RecipeListItem item)
    {
        if (item is null) return;
        if (item.Id == _recipeId) return;

        var page = ServiceHelper.Get<RecipeDetailPage>();
        if (page.BindingContext is RecipeDetailViewModel vm)
            await vm.Load(item.Id);
        await _navigation.PushAsync(page);
    }
}