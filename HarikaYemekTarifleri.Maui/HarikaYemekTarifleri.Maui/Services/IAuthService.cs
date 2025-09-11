//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HarikaYemekTarifleri.Maui.Services
//{
//    internal class IAuthService
//    {
//    }
//}

// Services/IAuthService.cs
using System.Net.Http.Json;

public interface IAuthService
{
    Task<bool> LoginAsync(string userName, string password);
    Task LogoutAsync();
    Task<bool> ChangePasswordAsync(string oldPwd, string newPwd);
}
