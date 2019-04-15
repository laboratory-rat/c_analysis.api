using Infrastructure.Interface.Manager;
using Infrastructure.Model.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRIdentityClient.Response;
using System.Threading.Tasks;

namespace MRCryptoCurrencyAnalysis.Controllers
{
    [Route("login")]
    public class LoginController : Controller
    {
        protected readonly ILoginManager _loginManager;

        public LoginController(ILoginManager loginManager)
        {
            _loginManager = loginManager;
        }

        /// <summary>
        /// Ping if user login is valid
        /// </summary>
        /// <returns></returns>
        [HttpGet("ping")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(ApiOkResult))]
        public IActionResult Ping()
        {
            return Ok(_loginManager.PingLogin());
        }

        [HttpPut("approve")]
        [ProducesResponseType(200, Type = typeof(UserLoginResponse))]
        [AllowAnonymous]
        public async Task<IActionResult> Approve([FromQuery] string token)
        {
            return Ok(await _loginManager.Approve(token));
        }

        [HttpPut]
        [ProducesResponseType(200, Type = typeof(ApiOkResult))]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            return Ok(await _loginManager.Logout());
        }

    }
}
