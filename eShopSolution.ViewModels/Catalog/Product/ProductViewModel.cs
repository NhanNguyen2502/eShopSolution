using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Product
{
    public class ProductViewModel
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public decimal Price { set; get; }
        public decimal OriginalPrice { set; get; }
        public int Stock { set; get; }
        public string ImgLink { get; set; }
        public DateTime DateCreated { set; get; }
        public List<string> Categories { get; set; }
    }
}