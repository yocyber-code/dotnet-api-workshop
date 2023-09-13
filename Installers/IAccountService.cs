using cmdev_dotnet_api.Entities;

namespace cmdev_dotnet_api.Installers
{
    public interface IAccountService
    {
        Task Register(Account account);
        Task Login(string username, string password);
    }
}
