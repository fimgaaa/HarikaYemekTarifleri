using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarikaYemekTarifleri.Maui.Helpers;
using HarikaYemekTarifleri.Maui.Models;
using HarikaYemekTarifleri.Maui.Pages;
using HarikaYemekTarifleri.Maui.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System;


namespace HarikaYemekTarifleri.Maui.ViewModels;

public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly IRecipeService _recipes;
    private readonly ICommentService _comments;
    private readonly INavigationService _navigation;
    private readonly IAuthService _auth;

    public ObservableCollection<CommentDto> Comments { get; } = new();
    public ObservableCollection<RecipeListItem> AuthorRecipes { get; } = new();
    [ObservableProperty]
    private RecipeDetail? recipe;

    [ObservableProperty]
    private string? newComment;
    //[ObservableProperty]
    //[NotifyCanExecuteChangedFor(nameof(EditCommand))]
    //private bool isOwner;
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
        //IsOwner = false;
        await Guard(async () =>
        {
            var detail = await _recipes.GetAsync(id);
            Recipe = detail;
            Comments.Clear();
            AuthorRecipes.Clear();
            if (detail is null)
            {
                return;
            }

            if (detail.Comments is not null)
            {
                foreach (var comment in detail.Comments)
                {
                    AddCommentToCollection(comment);
                }
            }

            var authorRecipes = await _recipes.GetByUserAsync(detail.UserId);
            foreach (var item in authorRecipes.Where(r => r.Id != detail.Id))
            {
                AuthorRecipes.Add(item);
            }
        });
    }

    [RelayCommand]
    private async Task AddComment()
    {
        if (string.IsNullOrWhiteSpace(NewComment))
        {
            Error = "Yorum boş olamaz.";
            return;
        }
        await Guard(async () =>
        {
            var dto = await _comments.AddAsync(_recipeId, NewComment!);
            if (dto is not null)
            {
                dto.UserName = string.IsNullOrWhiteSpace(dto.UserName)
                    ? _auth.CurrentUserName ?? string.Empty
                    : dto.UserName;

                AddCommentToCollection(dto);
                Recipe?.Comments.Add(dto);
                NewComment = string.Empty;
            }
        });
    }

    [RelayCommand]
    private async Task DeleteComment(CommentDto comment)
    {
        if (comment is null || !comment.IsMine)
        {
            return;
        }

        await Guard(async () =>
        {
            var deleted = await _comments.DeleteAsync(comment.Id);
            if (deleted)
            {
                Comments.Remove(comment);
                Recipe?.Comments.Remove(comment);
            }
        });
    }

    //partial void OnRecipeChanged(RecipeDetail? value)
    //{
    //    if (value is null)
    //    {
    //        IsOwner = false;
    //        return;
    //    }

    //    var owns = false;
    //    var currentUserId = _auth.CurrentUserId;
    //    if (currentUserId.HasValue)
    //    {
    //        owns = value.UserId == currentUserId.Value;
    //    }

    //    if (!owns)
    //    {
    //        var currentUserName = _auth.CurrentUserName;
    //        if (!string.IsNullOrWhiteSpace(currentUserName) &&
    //            !string.IsNullOrWhiteSpace(value.CreatedBy))
    //        {
    //            owns = string.Equals(value.CreatedBy, currentUserName, StringComparison.OrdinalIgnoreCase);
    //        }
    //    }

    //    IsOwner = owns;
    //}

    //[RelayCommand(CanExecute = nameof(CanEdit))]
    [RelayCommand]
    private async Task Edit()
    {
        var page = ServiceHelper.Get<RecipeEditPage>();
        if (page.BindingContext is RecipeEditViewModel vm)
        { await vm.Init(_recipeId);
        }
            
        await _navigation.PushAsync(page);
    }

    //private bool CanEdit() => IsOwner;

    [RelayCommand]
    private async Task OpenRecipe(RecipeListItem item)
    {
        if (item is null || item.Id == _recipeId)
        {
            return;
        }


        var page = ServiceHelper.Get<RecipeDetailPage>();
        if (page.BindingContext is RecipeDetailViewModel vm)
        {
            await vm.Load(item.Id);
        }

        await _navigation.PushAsync(page);
    }
    private void AddCommentToCollection(CommentDto comment)
    {
        if (comment is null)
        {
            return;
        }

        comment.IsMine = IsCurrentUsersComment(comment);
        Comments.Add(comment);
    }

    private bool IsCurrentUsersComment(CommentDto comment)
    {
        if (comment is null)
        {
            return false;
        }

        var currentUserName = _auth.CurrentUserName;
        if (string.IsNullOrWhiteSpace(currentUserName))
        {
            return false;
        }

        return string.Equals(comment.UserName, currentUserName, StringComparison.OrdinalIgnoreCase);
    }
}
