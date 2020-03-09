using System.Collections.Generic;
using System.Security.Claims;
using mservicesample.Membership.Core.Dtos.Requests;

namespace mservicesample.Membership.Core.Dtos.Responses
{
    public class UserTokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserDto User { get; set; }
        public IList<string> UserRoles { get; set; }
        public IList<Claim> UserClaims { get; set; }
    }
}
