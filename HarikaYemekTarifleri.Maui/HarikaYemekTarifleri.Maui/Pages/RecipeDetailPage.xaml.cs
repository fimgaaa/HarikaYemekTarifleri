using System.ComponentModel;
using HarikaYemekTarifleri.Maui.ViewModels;

namespace HarikaYemekTarifleri.Maui.Pages;

public partial class RecipeDetailPage : ContentPage
{
    private readonly RecipeDetailViewModel _vm;
    private readonly ToolbarItem _editToolbarItem;
    public RecipeDetailPage(RecipeDetailViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        _editToolbarItem = new ToolbarItem
        {
            Text = "Düzenle",
            BindingContext = vm
        };
        _editToolbarItem.SetBinding(ToolbarItem.CommandProperty, nameof(RecipeDetailViewModel.EditCommand));

        UpdateEditToolbarItem();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        //_vm.PropertyChanged += OnViewModelPropertyChanged;
        UpdateEditToolbarItem();
    }

    protected override void OnDisappearing()
    {
        //_vm.PropertyChanged -= OnViewModelPropertyChanged;
        base.OnDisappearing();
    }

    //private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    //{
    //    if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(RecipeDetailViewModel.IsOwner))
    //    {
    //        if (Dispatcher?.IsDispatchRequired ?? false)
    //        {
    //            Dispatcher.Dispatch(UpdateEditToolbarItem);
    //        }
    //        else
    //        {
    //            UpdateEditToolbarItem();
    //        }
    //    }
    //}

    private void UpdateEditToolbarItem()
    {
        //if (_vm.IsOwner)
        //{
        //    if (!ToolbarItems.Contains(_editToolbarItem))
        //    {
        //        ToolbarItems.Add(_editToolbarItem);
        //    }
        //}
        //else if (ToolbarItems.Contains(_editToolbarItem))
        if (!ToolbarItems.Contains(_editToolbarItem))
        {
            ToolbarItems.Add(_editToolbarItem);
            //ToolbarItems.Remove(_editToolbarItem);
        }
    }
}
