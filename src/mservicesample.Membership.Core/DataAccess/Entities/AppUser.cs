using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace mservicesample.Membership.Core.DataAccess.Entities
{
    public class AppUser : IdentityUser
    {
        // Add additional profile data for application users by adding properties to this class
        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}
