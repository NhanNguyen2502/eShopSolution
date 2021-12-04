using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Product.Manage
{
    public class GetProductPagingRequest : PagingRequestBase
    {
        public string Keyword { set; get; }
        public List<int> CategoryIds { get; set; }
    }
}
