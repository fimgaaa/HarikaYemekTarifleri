//using Microsoft.Extensions.Logging;

//namespace HarikaYemekTarifleri.Maui
//{
//    public static class MauiProgram
//    {
//        public static MauiApp CreateMauiApp()
//        {
//            var builder = MauiApp.CreateBuilder();
//            builder
//                .UseMauiApp<App>()
//                .ConfigureFonts(fonts =>
//                {
//                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
//                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
//                });

//#if DEBUG
//    		builder.Logging.AddDebug();
//#endif

//            return builder.Build();
//        }
//    }
//}
//using HarikaYemekTarifleri.Maui.Services;
//using CommunityToolkit.Mvvm; // NuGet: CommunityToolkit.Mvvm
//using HarikaYemekTarifleri.Maui.Pages;
//using HarikaYemekTarifleri.Maui.ViewModels;

//public static class MauiProgram
//{
//    public static MauiApp CreateMauiApp()
//    {
//        var builder = MauiApp.CreateBuilder();
//        builder.UseMauiApp<App>();

//        builder.Services.AddSingleton<ApiClient>();
//        builder.Services.AddSingleton<IAuthService, AuthService>();
//        builder.Services.AddSingleton<IRecipeService, RecipeService>();
//        builder.Services.AddSingleton<ICategoryService, CategoryService>();
//        builder.Services.AddSingleton<ICommentService, CommentService>();
//        builder.Services.AddSingleton<LoginViewModel>();
//        builder.Services.AddSingleton<RecipesViewModel>();
//        builder.Services.AddSingleton<RecipeEditViewModel>();

//        builder.Services.AddSingleton<LoginPage>();
//        builder.Services.AddSingleton<RecipesPage>();
//        builder.Services.AddSingleton<RecipeEditPage>();

//        // ViewModels & Pages kayıtları...
//        return builder.Build();
//    }
//}

using HarikaYemekTarifleri.Maui.Services;
using HarikaYemekTarifleri.Maui.Pages;
using HarikaYemekTarifleri.Maui.ViewModels;
using HarikaYemekTarifleri.Maui.Helpers;   // << ekledik

namespace HarikaYemekTarifleri.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Services
        builder.Services.AddSingleton<ApiClient>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
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

        // Pages
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<RecipesPage>();
        builder.Services.AddSingleton<RecipeEditPage>();
        builder.Services.AddSingleton<ProfilePage>();

        var app = builder.Build();

        // DI'ı global kullanmak için
        ServiceHelper.Initialize(app.Services);

        return app;
    }
}
