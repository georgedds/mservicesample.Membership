using mservicesample.Membership.Core.Dtos.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.Services
{
    public interface IRolesService
    {
        Task<bool> CreateAsync(RolesDto model);
        Task<bool> DeleteAsync(string roleid);
        Task<List<RolesDto>> GetAllRolesAsync();
        Task<RolesDto> FindByNameAsync(string rolename);
        Task<RolesDto> FindByIdAsync(string id);
        Task<bool> UpdateAsync(RolesDto role);
    }
}
