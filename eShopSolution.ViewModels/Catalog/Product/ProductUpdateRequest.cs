using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Product
{
    public class ProductUpdateRequest
    {
        public int Id { get; set; }
        public string Name { set; get; }

        [Range(0, int.MaxValue)]
        public decimal NeworOriginalPrice { get; set; }

        [Range(0, int.MaxValue)]
        public decimal NewPrice { get; set; }
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public string Description { set; get; }

        public string SeoTitle { set; get; }
    }
}