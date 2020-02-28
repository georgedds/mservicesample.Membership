using System.Collections.Generic;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.Services
{
    public interface IUserRoleService
    {
        Task<bool> AddUserRole(string userid, string roleid);
        Task<bool> RemoveRole(string userid, string roleid);
        Task<IList<string>> GetUserRoles(string userid);
    }
}
