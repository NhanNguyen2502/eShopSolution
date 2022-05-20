using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Product
{
    public class CategoryAssignRequest
    {
        public int id { get; set; }
        public List<SelectItem> Categories { get; set; } = new List<SelectItem>();
    }
}