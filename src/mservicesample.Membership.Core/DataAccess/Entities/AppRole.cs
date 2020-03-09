using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace mservicesample.Membership.Core.DataAccess.Entities
{
    public class AppRole : IdentityRole
    {
        //public string Description { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
