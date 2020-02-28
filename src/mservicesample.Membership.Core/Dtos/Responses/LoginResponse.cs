using mservicesample.Membership.Core.Dtos.Requests;
using System.Collections.Generic;
using System.Security.Claims;

namespace mservicesample.Membership.Core.Dtos.Responses
{
    public class LoginResponse
    {
        public UserDto User { get; set; }
        public IList<string> UserRoles { get; set; }
        public IList<Claim> UserClaims { get; set; }
        public UserTokenDto UserToken { get; set; }
    }
}
