using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mservicesample.Membership.Core.DataAccess.Entities;

namespace mservicesample.Membership.Core.DataAccess.Identity
{
    public class AppIdentityDbContext : //IdentityDbContext<AppUser, AppRole, string>

        IdentityDbContext<AppUser, AppRole, string, IdentityUserClaim<string>,
            AppUserRole, IdentityUserLogin<string>,
            IdentityRoleClaim<string>, IdentityUserToken<string>>


    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {
        }
    }
}
