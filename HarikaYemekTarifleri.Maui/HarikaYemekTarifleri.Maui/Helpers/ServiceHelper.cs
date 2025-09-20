using System.Runtime.ConstrainedExecution;

namespace HarikaYemekTarifleri.Maui.Helpers;

public static class ServiceHelper
{
    //ServiceHelper = global servis erişim noktası

    //Initialize() = uygulama başlarken DI Container’ı kaydeder

    //Get<T>() = kayıtlı servisi her yerden almanı sağlar

    //Kullanım: sayfa açmak, ViewModel almak, DI’dan servis çözmek
    public static IServiceProvider Services { get; private set; } = default!;
    public static void Initialize(IServiceProvider services) => Services = services;
    public static T Get<T>() where T : notnull => Services.GetRequiredService<T>();
}
