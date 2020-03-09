using mservicesample.Membership.Core.Dtos.Requests;
using mservicesample.Membership.Core.Helpers;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.Services
{
    public interface IUserService
    {
        Task<UserDto> Register(UserDto user);
        Task<Pager.PagedResult<UserDto>> GetAllUsers(Pager.PagingParams pager);
        Task<bool> Delete(string userid);
        Task<UserDto> Edit(UserDto user);
        Task<UserDto> GetById(string userid);
    }
}
