using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Common
{
    public class PagedResult<T> : PaginationBase
    {
        public List<T> Items { set; get; }
    }
}