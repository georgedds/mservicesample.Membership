using mservicesample.Membership.Core.DataAccess.Entities;
using mservicesample.Membership.Core.Helpers;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.DataAccess.Repositories
{
    public interface IUserDetailsRepository : IRepository<UserDetails>
    {
        Task<UserDetails> Create(UserDetails user);
        Task<UserDetails> FindByName(string userName);
        Task<UserDetails> FindById(string id);
        Task<bool> CheckPassword(UserDetails user, string password);
        Task<Pager.PagedResult<UserDetails>> ListPaged(Pager.PagingParams pager);
        Task<UserDetails> Add(UserDetails entity);
        Task<UserDetails> Update(UserDetails entity);
        Task<bool> Delete(string userid);
    }
}
