using HarikaYemekTarifleri.Maui.ViewModels;

namespace HarikaYemekTarifleri.Maui.Pages;

public partial class ChangePasswordPage : ContentPage
{
    public ChangePasswordPage(ChangePasswordViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}