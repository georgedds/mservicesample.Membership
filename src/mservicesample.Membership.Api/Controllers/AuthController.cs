using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using mservicesample.Membership.Core.Dtos.Requests;
using mservicesample.Membership.Core.Services;
using mservicesample.Membership.Core.Settings;

namespace mservicesample.Membership.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly AppSettings _authSettings;

        public AuthController(ILoginService loginService, IOptions<AppSettings> authSettings)
        {
            _loginService = loginService;
            _authSettings = authSettings.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginrs = await _loginService.Login(request);
            return Ok(loginrs);
        }


        [HttpPost("refreshtoken")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rs = await _loginService.RefreshToken(request);
            return Ok(rs);
        }
    }
}
