using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mservicesample.Membership.Core.Dtos.Requests;
using mservicesample.Membership.Core.Helpers;
using mservicesample.Membership.Core.Services;

namespace mservicesample.Membership.Api.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountsController(IUserService registerUserUseCase)
        {
            _userService = registerUserUseCase;
        }

        // POST api/accounts
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post([FromBody] UserDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _userService.Register(request));
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] Pager.PagingParams paging)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _userService.GetAllUsers(paging));
        }

        [HttpGet]
        [Route("{userid}")]
        public async Task<ActionResult> GetById(string userid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _userService.GetById(userid));
        }

        [HttpPut]
        public async Task<ActionResult> Edit([FromBody] UserDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _userService.Edit(request));
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(string userid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _userService.Delete(userid));
        }
    }
}
