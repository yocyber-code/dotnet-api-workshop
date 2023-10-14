using cmdev_dotnet_api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace cmdev_dotnet_api.interfaces
{
    public interface IAccountService
    {
        Task Register(Account account);
        Task<ActionResult> Login(string username, string password);
        Account GetInfo(string accessToken);
    }
}
