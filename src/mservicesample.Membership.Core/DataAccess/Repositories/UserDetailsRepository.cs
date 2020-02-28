using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mservicesample.Membership.Core.DataAccess.Entities;
using mservicesample.Membership.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mservicesample.Membership.Core.Middleware;

namespace mservicesample.Membership.Core.DataAccess.Repositories
{
     public class UserDetailsRepository : EfRepository<UserDetails>, IUserDetailsRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;


        public UserDetailsRepository(UserManager<AppUser> userManager, IMapper mapper, AppDbContext appDbContext) : base(appDbContext)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserDetails> Create(UserDetails user)
        {

            var appUser = new AppUser { Email = user.Email, UserName = user.UserName };
            var identityResult = await _userManager.CreateAsync(appUser, user.PasswordHash);

            if (!identityResult.Succeeded)
            {
                throw new AppException(String.Join("  ,  ", identityResult.Errors.Select(e => e.Code + " : " + e.Description)));
            }

            var userdetails = new UserDetails(user.FirstName, user.LastName, appUser.Id, appUser.UserName, user.Comments, user.Email);
            _appDbContext.UserDetails.Add(userdetails);
            await _appDbContext.SaveChangesAsync();
            return user;
        }


        public async Task<UserDetails> FindByName(string userName)
        {
            var appUser =
                await _appDbContext.UserDetails.FirstOrDefaultAsync(x => string.Equals(x.UserName, userName, StringComparison.CurrentCultureIgnoreCase));
            return appUser;
        }

        public async Task<UserDetails> FindById(string id)
        {
            var appUser =
                await _appDbContext.UserDetails.Include(x=>x.RefreshTokens).FirstOrDefaultAsync(x => x.IdentityId == id);
            return appUser;
        }

        public async Task<bool> CheckPassword(UserDetails user, string password)
        {
            return await _userManager.CheckPasswordAsync(_mapper.Map<AppUser>(user), password);
        }

        public async Task<Pager.PagedResult<UserDetails>> ListPaged(Pager.PagingParams pager)
        {
            return await _appDbContext.UserDetails.GetPaged(pager.PageNumber, pager.PageSize);
        }

        public async Task<UserDetails> GetById(int id)
        {
            return await _appDbContext.UserDetails.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<UserDetails>> ListAll()
        {
            return _appDbContext.UserDetails.ToListAsync();
        }

        public async Task<UserDetails> Add(UserDetails entity)
        {
            _appDbContext.UserDetails.Add(entity);
            var rs = await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<UserDetails> Update(UserDetails entity)
        {
            var identityUser = await _userManager.FindByIdAsync(entity.IdentityId);
            if (identityUser == null || identityUser.Id != entity.IdentityId) return null;

            using (var trx = _appDbContext.Database.BeginTransaction())
            {

                if (!string.IsNullOrEmpty(entity.Email) && identityUser.Email.ToLower() != entity.Email.ToLower())
                {
                    await _userManager.SetEmailAsync(identityUser, entity.Email);
                }


                var currentrec = _appDbContext.UserDetails.FirstOrDefault(x => x.IdentityId == entity.IdentityId);
                currentrec.Email = entity.Email;
                currentrec.Comments = entity.Comments;
                currentrec.FirstName = entity.FirstName;
                currentrec.LastName = entity.LastName;

                var rs = await _appDbContext.SaveChangesAsync();
                trx.Commit();
                return entity;
            }
        }

        public async Task<bool> Delete(string userid)
        {
            var identityUser = await _userManager.FindByIdAsync(userid);
            if (identityUser == null || identityUser.Id != userid) return false;
            using (var trx = _appDbContext.Database.BeginTransaction())
            {
                var record = await _appDbContext.UserDetails.FirstOrDefaultAsync(x => x.IdentityId == userid);
                var refreshtoken = await _appDbContext.RefreshTokens.Where(x => x.UserId == record.Id).ToListAsync();

                _appDbContext.RefreshTokens.RemoveRange(refreshtoken);

                _appDbContext.UserDetails.Remove(record);
                var rs = await _appDbContext.SaveChangesAsync();
                await _userManager.DeleteAsync(identityUser);

                trx.Commit();
                return true;
            }
        }
    }
}
