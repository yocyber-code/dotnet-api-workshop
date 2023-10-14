using cmdev_dotnet_api.DTOs.Account;
using cmdev_dotnet_api.Entities;
using cmdev_dotnet_api.interfaces;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace cmdev_dotnet_api.Controllers
{
    [ApiController] //date annotation
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountsController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromForm] RegisterRequest account)
        {
            Account newAccount = account.Adapt<Account>();
            await accountService.Register(newAccount);
            return Created(nameof(Register), account);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string?>> Login([FromForm] LoginRequest account)
        {
            return await accountService.Login(account.Username, account.Password);
        }

        [HttpGet("info")]
        public async Task<ActionResult<Account?>> GetInfo()
        {
            string? accessToken = await HttpContext.GetTokenAsync("access_token");
            if (accessToken == null)
            {
                return Unauthorized();
            }
            Account account = accountService.GetInfo(accessToken);
            return Ok(new { username = account.Username, role = account.Role.Name });
        }
    }
}
