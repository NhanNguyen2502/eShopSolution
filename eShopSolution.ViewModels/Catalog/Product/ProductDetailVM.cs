using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Product
{
    public class ProductDetailVM
    {
        public int Id { get; set; }
        public string Name { set; get; }
        public decimal Price { set; get; }
        public decimal OrOriginalPrice { set; get; }
        public int Stock { set; get; }
        public string DesDescription { set; get; }
        public string LanguageId { get; set; }
        public int CategoryId { get; set; }
        public string Details { set; get; }
        public string SeoDescription { get; set; }
        public string SeoTitle { get; set; }
        public string SeoAlias { get; set; }
        public DateTime DateCreated { get; set; }

        public List<string> Categories { get; set; } //= new List<string>();
    }
}