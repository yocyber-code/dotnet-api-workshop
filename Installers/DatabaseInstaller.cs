using cmdev_dotnet_api.Data;
using Microsoft.EntityFrameworkCore;

namespace cmdev_dotnet_api.Installers
{
    public class DatabaseInstaller : IInstallers
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
                           options.UseSqlServer(configuration.GetConnectionString("ConnectionSQLServer")));
        }
    }
}
