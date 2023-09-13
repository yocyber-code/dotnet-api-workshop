using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace cmdev_dotnet_api.Installers
{
    public class JWTInstaller : IInstallers
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            JwtSettings jwtSettings = new JwtSettings(configuration);
            if (!jwtSettings.valid())
            {
                throw new Exception("JWT Settings not valid");
            }
            configuration.Bind(nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }

    public class JwtSettings
    {
        public JwtSettings(IConfiguration configuration)
        {
            Key = configuration["JwtSettings:Key"] ?? "";
            Issuer = configuration["JwtSettings:Issuer"] ?? "";
            Audience = configuration["JwtSettings:Audience"] ?? "";
            Expiration = configuration["JwtSettings:Expiration"] ?? "";
        }

        public bool valid()
        {
            if (Key == "" || Issuer == "" || Audience == "" || Expiration == "")
            {
                return false;
            }
            return true;
        }

        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string Expiration { get; set; } = null!;
    }
}
