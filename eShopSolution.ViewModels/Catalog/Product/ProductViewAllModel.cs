using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Product
{
    public class ProductViewAllModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductOriginalPrice { get; set; }
        public int ProductStock { get; set; }
        public int ProductViewCount { get; set; }
        public string Description { get; set; }
        public string Language { set; get; }

    }
}
