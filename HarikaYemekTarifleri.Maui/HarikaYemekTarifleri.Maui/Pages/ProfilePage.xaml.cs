using HarikaYemekTarifleri.Maui.ViewModels;

namespace HarikaYemekTarifleri.Maui.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _vm;
    //Constructor ile bağlama
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        _vm = vm; // readonly alan burada dolduruluyor
        BindingContext = vm; // XAML bağlamı için de aynı vm atanıyor
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Load();
    }
}