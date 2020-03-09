using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mservicesample.Membership.Core.DataAccess.Entities;
using mservicesample.Membership.Core.Dtos.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.Services
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;

        public RolesService(RoleManager<AppRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(RolesDto model)
        {
            var role = new AppRole();
            var map = _mapper.Map(model, role);
            var rs = await _roleManager.CreateAsync(map);
            return rs.Succeeded;
        }
        public async Task<bool> DeleteAsync(string roleid)
        {
            var role = await _roleManager.FindByIdAsync(roleid);
            var rs = await _roleManager.DeleteAsync(role);
            return rs.Succeeded;
        }

        public async Task<List<RolesDto>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();
            var map = _mapper.Map<List<RolesDto>>(roles);
            return map;
        }

        public async Task<RolesDto> FindByNameAsync(string rolename)
        {
            var role = await _roleManager.FindByNameAsync(rolename);
            var map = _mapper.Map<RolesDto>(role);
            return map;
        }

        public async Task<RolesDto> FindByIdAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var map = _mapper.Map<RolesDto>(role);
            return map;
        }

        public async Task<bool> UpdateAsync(RolesDto role)
        {
            var rolemapp = _mapper.Map<AppRole>(role);
            //for some strange reason it doesn not work since it give a tracking error...
            var rs = await _roleManager.UpdateAsync(rolemapp);
            return rs.Succeeded;
        }
    }
}
