using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace mservicesample.Membership.Core.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public TokenGenerator()
        {
            if (_jwtSecurityTokenHandler == null)
                _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }
        public string GenerateToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            return ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateLifetime = false // we check expired tokens here
            });
        }
    }
}
