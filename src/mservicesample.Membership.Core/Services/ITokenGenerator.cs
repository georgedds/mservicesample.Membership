using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace mservicesample.Membership.Core.Services
{
    public interface ITokenGenerator
    {
        string GenerateToken(int size = 32);
        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
        ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey);
    }
}
