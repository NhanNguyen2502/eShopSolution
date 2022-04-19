using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Product
{
    public class ProductCreateRequest
    {
        [Required]
        public string Name { set; get; }
        [Required]
        public decimal Price { set; get; }
        [Required]
        public decimal OrOriginalPrice { set; get; }
        [Required]
        public int Stock { set; get; }
        [Required]
        public string DesDescription { set; get; }
        [Required]
        public string LanguageId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string Details { set; get; }
        public string SeoDescription { get; set; }      
        public string SeoTitle { get; set; }
        public string SeoAlias { get; set; }

        [Required]
        public IFormFile ThumbnailImage { set; get; }


    }
}
