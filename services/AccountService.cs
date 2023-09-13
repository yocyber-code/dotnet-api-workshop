using cmdev_dotnet_api.Data;
using cmdev_dotnet_api.Entities;
using cmdev_dotnet_api.Installers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace cmdev_dotnet_api.services
{
    public class AccountService : IAccountService
    {
        private readonly DatabaseContext databaseContext;
        private readonly JwtSettings jwtSettings;

        public AccountService(DatabaseContext databaseContext,JwtSettings jwtSettings)
        {
            this.databaseContext = databaseContext;
            this.jwtSettings = jwtSettings;
        }

        public async Task<ActionResult> Login(string username, string password)
        {
            Account? existAccounts = await databaseContext.Accounts.Include(a => a.Role)
                .SingleOrDefaultAsync(x => x.Username == username);
            if (existAccounts == null)
            {
                return new UnauthorizedResult();
            }
            bool isPasswordVerify = VerifyPassword(existAccounts.Password, password);
            if (!isPasswordVerify)
            {
                return new UnauthorizedResult();
            }
            return new OkObjectResult(new
            {
                access_token = GenerateJwtToken(existAccounts)
            });
        }

        public async Task Register(Account account)
        {
            Account? existAccounts = await databaseContext.Accounts.SingleOrDefaultAsync(x => x.Username == account.Username);
            if (existAccounts != null)
            {
                throw new Exception("Username is already taken");
            }
            account.Password = CreatePasswordHash(account.Password);
            databaseContext.Accounts.Add(account);
            await databaseContext.SaveChangesAsync();
        }

        private string CreatePasswordHash(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, 10000, 258 / 8));
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        private bool VerifyPassword(string hashed, string password)
        {
            string[] parts = hashed.Split('.', 2);
            if (parts.Length != 2)
            {
                return false;
            }
            byte[] salt = Convert.FromBase64String(parts[0]);
            string hash = parts[1];

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password, salt, KeyDerivationPrf.HMACSHA512, 10000, 258 / 8));

            return hash == hashedPassword;
        }

        private string GenerateJwtToken(Account account)
        {
            //key is case sensitive
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Username),
                new Claim("role", account.Role.Name),
            };

            DateTime expires = DateTime.Now.AddDays(Convert.ToDouble(jwtSettings.Expiration));
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                                            issuer : jwtSettings.Issuer,
                                            audience : jwtSettings.Audience,
                                            claims : claims,
                                            expires: expires,
                                            signingCredentials: credentials
                                         );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        public Account GetInfo(string accessToken)
        {
            JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

            //key is case sensitive
            string username = token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
            string role = token.Claims.First(claim => claim.Type == "role").Value;

            Account account = new Account
            {
                Username = username,
                Role = new Role
                {
                    Name = role
                }
            };

            return account;
        }
    }
}
