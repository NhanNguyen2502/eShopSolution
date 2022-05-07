using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Common
{
    public class PaginationBase
    {
        public int TotalRecord { set; get; }
        public int pageIndex { set; get; }
        public int pagSize { set; get; }

        public int pagecount
        {
            get
            {
                var pagecount = (double)TotalRecord / pagSize;
                return (int)Math.Ceiling(pagecount);
            }
        }
    }
}