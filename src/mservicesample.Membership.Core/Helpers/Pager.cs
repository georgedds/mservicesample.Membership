using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.Helpers
{
    public class Pager
    {
        public abstract class PagedResultBase
        {
            public int CurrentPage { get; set; }
            public int PageCount { get; set; }
            public int PageSize { get; set; }
            public int RowCount { get; set; }

            public int FirstRowOnPage
            {

                get { return (CurrentPage - 1) * PageSize + 1; }
            }

            public int LastRowOnPage
            {
                get { return Math.Min(CurrentPage * PageSize, RowCount); }
            }
        }

        public class PagedResult<T> : PagedResultBase where T : class
        {
            public Task<List<T>> Results { get; set; }

            //public PagedResult()
            //{
            //    Results = new Task<List<T>>();
            //}
        }

        public class PagingParams
        {
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 5;
        }
    }
}
