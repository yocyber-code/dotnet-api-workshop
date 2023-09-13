using System.ComponentModel.DataAnnotations;

namespace cmdev_dotnet_api.DTOs.Account
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
