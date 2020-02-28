using mservicesample.Membership.Core.DataAccess.Entities;
using mservicesample.Membership.Core.Dtos.Requests;
using mservicesample.Membership.Core.Dtos.Responses;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.Services
{
    public interface ILoginService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<UserTokenDto> RefreshToken(RefreshTokenRequest request);
        Task<string> GenerateUserJwtToken(AppUser user);
    }
}
