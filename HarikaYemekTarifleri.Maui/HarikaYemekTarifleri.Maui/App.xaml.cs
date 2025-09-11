namespace HarikaYemekTarifleri.Maui;

public partial class App : Application
{
    public App(Pages.LoginPage loginPage)
    {
        InitializeComponent();
        MainPage = new NavigationPage(loginPage);
    }
}
