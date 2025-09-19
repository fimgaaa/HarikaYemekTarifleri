using HarikaYemekTarifleri.Maui.Services;
using HarikaYemekTarifleri.Maui.Pages;
using HarikaYemekTarifleri.Maui.ViewModels;
using HarikaYemekTarifleri.Maui.Helpers;

namespace HarikaYemekTarifleri.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>(); //Uygulamanın başlangıç sınıfı olan App.xaml.cs dosyasını kullanır, uygulama buradan ayağa kalkar.
                                   //Bu uygulamanın giriş noktası App sınıfı olsun. Uygulamayı bu sınıftan başlat.

        // Services
        builder.Services.AddSingleton<ApiClient>();
        builder.Services.AddSingleton<IAuthService, AuthService>(); //Biri IAuthService isterse, AuthService ver.
        builder.Services.AddSingleton<IRecipeService, RecipeService>();
        builder.Services.AddSingleton<ICategoryService, CategoryService>();
        builder.Services.AddSingleton<ICommentService, CommentService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IUserService, UserService>();

        // ViewModels
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<RecipesViewModel>();
        builder.Services.AddSingleton<RecipeEditViewModel>();
        builder.Services.AddSingleton<ProfileViewModel>();
        builder.Services.AddSingleton<RegisterViewModel>();
        builder.Services.AddTransient<RecipeDetailViewModel>();
        builder.Services.AddSingleton<ChangePasswordViewModel>();

        // Pages
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<RecipesPage>();
        builder.Services.AddSingleton<RecipeEditPage>();
        builder.Services.AddSingleton<ProfilePage>();
        builder.Services.AddSingleton<RegisterPage>();
        builder.Services.AddTransient<RecipeDetailPage>();
        builder.Services.AddSingleton<ChangePasswordPage>();

        var app = builder.Build();

        // DI'ı global kullanmak için
        ServiceHelper.Initialize(app.Services);

        return app;
    }
}
