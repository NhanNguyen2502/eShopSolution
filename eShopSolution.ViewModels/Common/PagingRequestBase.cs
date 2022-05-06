using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.Common
{
    public class PagingRequestBase
    {
        [Range(1, Int32.MaxValue)]
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; }
    }
}