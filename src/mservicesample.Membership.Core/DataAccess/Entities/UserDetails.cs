using System;
using System.Collections.Generic;
using System.Linq;

namespace mservicesample.Membership.Core.DataAccess.Entities
{
    public class UserDetails : BaseEntity
    {
        public string FirstName { get; set; } // EF migrations require at least private setter - won't work on auto-property
        public string LastName { get; set; }
        public string IdentityId { get; private set; }
        public string UserName { get; private set; } // Required by automapper
        public string Email { get; set; }
        public string PasswordHash { get; private set; }
        public string Comments { get; set; }

        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        internal UserDetails() { /* Required by EF */ }

        internal UserDetails(string firstName, string lastName, string identityId, string userName, string comments, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            IdentityId = identityId;
            UserName = userName;
            Comments = comments;
            Email = email;
        }

        public bool HasValidRefreshToken(string refreshToken)
        {
            return _refreshTokens.Any(rt => rt.Token.ToLower() == refreshToken.ToLower() && rt.Active);
        }

        public void AddRefreshToken(string token, int userId, string remoteIpAddress, double daysToExpire = 5)
        {
            _refreshTokens.Add(new RefreshToken(token, DateTime.UtcNow.AddDays(daysToExpire), userId, remoteIpAddress));
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            _refreshTokens.Remove(_refreshTokens.First(t => t.Token == refreshToken));
        }
    }
}
