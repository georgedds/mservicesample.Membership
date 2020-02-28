using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using mservicesample.Membership.Core.DataAccess.Entities;
using mservicesample.Membership.Core.DataAccess.Repositories;
using mservicesample.Membership.Core.Dtos.Requests;
using mservicesample.Membership.Core.Dtos.Responses;
using mservicesample.Membership.Core.Helpers;
using mservicesample.Membership.Core.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using mservicesample.Membership.Core.Middleware;

namespace mservicesample.Membership.Core.Services
{
     public class LoginService : ILoginService
    {
        private readonly IUserDetailsRepository _userDetailsRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IMapper _mapper;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AppSettings _appSettings;

        public LoginService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IUserDetailsRepository userDetailsRepository, IOptions<JwtIssuerOptions> jwtOptions, IMapper mapper, ITokenGenerator tokenGenerator, IOptions<AppSettings> appSettings)
        {
            _userDetailsRepository = userDetailsRepository;
            _jwtOptions = jwtOptions.Value;
            _mapper = mapper;
            _tokenGenerator = tokenGenerator;
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _roleManager = roleManager;


            if (_jwtSecurityTokenHandler == null)
                _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() -
                                 new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);

        public async Task<string> GenerateUserJwtToken(AppUser user)
        {
            // Get valid claims and pass them into JWT
            var claims = await GetValidClaims(user);
            var secretkey = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Subject = new ClaimsIdentity(claims: claims),
                NotBefore = _jwtOptions.NotBefore,
                Expires =  _jwtOptions.Expiresinminutes > 0 ? _jwtOptions.IssuedAt.Add(TimeSpan.FromMinutes(_jwtOptions.Expiresinminutes)) :  _jwtOptions.Expiration,
                //signingCredentials: _jwtOptions.SigningCredentials
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretkey),
                    SecurityAlgorithms.HmacSha256Signature)
            };


            // Create the JWT security token and encode it.
            //var tokenDescriptor = new JwtSecurityToken(
            //        issuer: _jwtOptions.Issuer,
            //        audience: _jwtOptions.Audience,
            //        claims: claims,
            //        notBefore: _jwtOptions.NotBefore,
            //        expires: _jwtOptions.Expiration,
            //        //signingCredentials: _jwtOptions.SigningCredentials
            //        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretkey), SecurityAlgorithms.HmacSha256Signature)
            //        );

            var token = _jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            var tokenString = _jwtSecurityTokenHandler.WriteToken(token);

            return tokenString;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {

            var result = await PasswordSignInAsync(request.UserName, request.Password, true, false);
            if (result == Enumerations.SignInStatus.Success)
            {
                var user = await _userManager.FindByNameAsync(request.UserName);

                var tokenString = await GenerateUserJwtToken(user);

                var userdetails = await _userDetailsRepository.FindById(user.Id);
                var claims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);

                var refreshToken = _tokenGenerator.GenerateToken();
                userdetails.AddRefreshToken(refreshToken, userdetails.Id, request.RemoteIpAddress, 10);
                await _userDetailsRepository.Update(userdetails);


                var rs = new LoginResponse
                {
                    UserToken = new UserTokenDto {RefreshToken = refreshToken, AccessToken = tokenString},
                    User = _mapper.Map<UserDto>(userdetails),
                    UserClaims = claims,
                    UserRoles = roles
                };

                return rs;
            }
            throw new AppException("Invalid login");
        }

        public async Task<UserTokenDto> RefreshToken(RefreshTokenRequest request)
        {
            var cp = _tokenGenerator.GetPrincipalFromToken(request.AccessToken, _appSettings.Secret);

            // invalid token/signing key was passed and we can't extract user claims
            if (cp != null)
            {
                var _options = new IdentityOptions();
                //todo check issue with two sampe claim types
                var id = cp.Claims.Last(c => c.Type == _options.ClaimsIdentity.UserIdClaimType);
                var userdet = await _userDetailsRepository.FindById(id.Value);

                var user = await _userManager.FindByNameAsync(userdet.UserName);
                
                var claims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                

                //todo check issue with valid refresh token
                if (userdet.HasValidRefreshToken(request.RefreshToken))
                {
                    var appuser = await _userManager.FindByIdAsync(userdet.IdentityId);
                    var jwtToken = await GenerateUserJwtToken(appuser);
                    var refreshToken = _tokenGenerator.GenerateToken();
                    userdet.RemoveRefreshToken(request.RefreshToken); // delete the token we've exchanged
                    userdet.AddRefreshToken(refreshToken, userdet.Id, string.Empty); // add the new one
                    await _userDetailsRepository.Update(userdet);
                    return new UserTokenDto{RefreshToken = refreshToken, AccessToken =  jwtToken,UserClaims = claims,UserRoles = roles,User =  _mapper.Map<UserDto>(userdet)};
                }
            }

            throw new AppException("Invalid Token!");
        }



        private async Task<List<Claim>> GetValidClaims(AppUser user)
        {
            var _options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
                new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName)
            };
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userClaims);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }

        private async Task<Enumerations.SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Enumerations.SignInStatus.Failure;
            }
            if (await _userManager.IsLockedOutAsync(user))
            {
                return Enumerations.SignInStatus.LockedOut;
            }
            if (await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.ResetAccessFailedCountAsync(user);

                return Enumerations.SignInStatus.Success;
                //await SignInOrTwoFactor(user, isPersistent).WithCurrentCulture();
            }
            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await _userManager.AccessFailedAsync(user);
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return Enumerations.SignInStatus.LockedOut;
                }
            }
            return Enumerations.SignInStatus.Failure;
        }

        //todo add validation logic later
        //public IdentityLoginResult ValidateIdentityUser(string username, string password, bool shouldLockout)
        //{
        //    var user = _userManager.FindByName(username);
        //    if (user == null)
        //        return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.MemberNotExists };

        //    if (_userManager.IsLockedOut(user.Id))
        //        return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.IsLockedOut };


        //    if (shouldLockout)
        //    {
        //         _userManager.AccessFailedAsync(user);
        //        if (_userManager.IsLockedOutAsync(user).Result)
        //            return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.IsLockedOut };
        //    }

        //    return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.WrongPassword };
        //}

        //private async Task<SignInStatus> SignInOrTwoFactor(TUser user, bool isPersistent)
        //{
        //    var id = Convert.ToString(user.Id);
        //    if (await UserManager.GetTwoFactorEnabledAsync(user.Id).WithCurrentCulture()
        //        && (await UserManager.GetValidTwoFactorProvidersAsync(user.Id).WithCurrentCulture()).Count > 0
        //        && !await AuthenticationManager.TwoFactorBrowserRememberedAsync(id).WithCurrentCulture())
        //    {
        //        var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
        //        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
        //        AuthenticationManager.SignIn(identity);
        //        return SignInStatus.RequiresVerification;
        //    }
        //    await SignInAsync(user, isPersistent, false).WithCurrentCulture();
        //    return SignInStatus.Success;
        //}
    }
}
