using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mservicesample.Membership.Core.Dtos.Requests;
using mservicesample.Membership.Core.Services;

namespace mservicesample.Membership.Api.Controllers
{
    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        private readonly IRolesService _rolesService;
        private readonly IUserRoleService _userRoleService;

        public RolesController(IRolesService rolesService, IUserRoleService userRoleService)
        {
            _rolesService = rolesService;
            _userRoleService = userRoleService;
        }

        #region SystemRoles

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var rs = await _rolesService.GetAllRolesAsync();
            return Ok(rs);
        }

        [HttpGet("roleid/{roleid}")]
        public async Task<ActionResult> GetById(string roleid)
        {
            var rs = await _rolesService.FindByIdAsync(roleid);
            return Ok(rs);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult> GetByName(string name)
        {
            var rs = await _rolesService.FindByNameAsync(name);
            return Ok(rs);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RolesDto role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rs = await _rolesService.CreateAsync(role);
            return Ok(rs);
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] RolesDto role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rs = await _rolesService.UpdateAsync(role);
            return Ok(rs);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(string roleid)
        {
            if (string.IsNullOrEmpty(roleid))
            {
                return BadRequest("Input parameter missing");
            }

            var rs = await _rolesService.DeleteAsync(roleid);
            return Ok(rs);
        }

        #endregion SystemRoles

        #region UserRoles

        [HttpGet("userrole/{userid}")]
        public async Task<ActionResult> GetUserRole(string userid)
        {
            var rs = await _userRoleService.GetUserRoles(userid);
            return Ok(rs);
        }


        [HttpPost("userrole")]
        public async Task<ActionResult> AddUserRole([FromBody] UserRoleRequest request)
        {
            var rs = await _userRoleService.AddUserRole(request.UserId, request.RoleId);
            return Ok(rs);
        }

        [HttpDelete("userrole")]
        public async Task<ActionResult> DeleteUserRole(string userId, string roleId)
        {
            var request = new UserRoleRequest {RoleId = roleId, UserId = userId};
            var rs = await _userRoleService.RemoveRole(request.UserId, request.RoleId);
            return Ok(rs);
        }

        #endregion UserRoles
    }
}
