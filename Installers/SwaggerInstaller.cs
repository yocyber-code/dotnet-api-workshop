using Microsoft.OpenApi.Models;

namespace cmdev_dotnet_api.Installers
{
    public class SwaggerInstaller : IInstallers
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "cmdev_dotnet_api", Version = "v1" });
            });
        }
    }   
}
