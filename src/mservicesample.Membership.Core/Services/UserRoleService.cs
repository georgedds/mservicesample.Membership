using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mservicesample.Membership.Core.Middleware;
using mservicesample.Membership.Core.DataAccess.Entities;

namespace mservicesample.Membership.Core.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserRoleService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<bool> AddUserRole(string userid, string roleid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null) return false;
            var role = await _roleManager.FindByIdAsync(roleid);
            if (role != null)
            {
                var rs = await _userManager.AddToRoleAsync(user, role.Name);
                return rs.Succeeded;
            }
            return false;
        }

        public async Task<bool> RemoveRole(string userid, string roleid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null) return false;
            var role = await _roleManager.FindByIdAsync(roleid);
            if (role != null)
            {
                var rs = await _userManager.RemoveFromRoleAsync(user, role.Name);
                return rs.Succeeded;
            }
            return false;
        }

        public async Task<IList<string>> GetUserRoles(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null) throw new AppException("User is not valid");
            {
                var rs = await _userManager.GetRolesAsync(user);
                
                return rs;
            }
        }
    }
}
