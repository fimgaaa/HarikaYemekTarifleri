using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Services;
using System.Collections.ObjectModel;

namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly IRecipeService _recipes;
    private readonly ICommentService _comments;

    public ObservableCollection<string> Comments { get; } = new();

    [ObservableProperty] private RecipeDetail? recipe;
    [ObservableProperty] private string? newComment;
    private int _recipeId;

    public RecipeDetailViewModel(IRecipeService recipes, ICommentService comments)
    {
        _recipes = recipes;
        _comments = comments;
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
            var ok = await _comments.AddAsync(_recipeId, NewComment!);
            if (ok)
            {
                Comments.Add(NewComment!);
                NewComment = string.Empty;
            }
        });
    }
}