using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Helpers;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Pages;
using HarikaYemekTarifleri.Maui.Services;
using System.Collections.ObjectModel;


namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly IRecipeService _recipes;
    private readonly ICommentService _comments;
    private readonly INavigationService _navigation;

    public ObservableCollection<CommentDto> Comments { get; } = new();

    [ObservableProperty] private RecipeDetail? recipe;
    [ObservableProperty] private string? newComment;
    private int _recipeId;

    public RecipeDetailViewModel(IRecipeService recipes, ICommentService comments, INavigationService navigation)
    {
        _recipes = recipes;
        _comments = comments;
        _navigation = navigation;
    }

    public async Task Load(int id)
    {
        _recipeId = id;
        await Guard(async () =>
        {
            Recipe = await _recipes.GetAsync(id);
            Comments.Clear();
            if (Recipe?.Comments is not null)
            {
                foreach (var c in Recipe.Comments)
                    Comments.Add(c);
            }
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

    [RelayCommand]
    private async Task Edit()
    {
        var page = ServiceHelper.Get<RecipeEditPage>();
        if (page.BindingContext is RecipeEditViewModel vm)
            await vm.Init(_recipeId);
        await _navigation.PushAsync(page);
    }
}