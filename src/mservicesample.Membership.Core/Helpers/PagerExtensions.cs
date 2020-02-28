using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.Helpers
{
    public static class PagerExtensions
    {
        public static async Task<Pager.PagedResult<T>> GetPaged<T>(this IQueryable<T> query, int page, int pageSize) where T : class
        {
            var result = new Pager.PagedResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();


            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = Task.FromResult(await query.Skip(skip).Take(pageSize).ToListAsync());

            return result;
        }
    }
}
