﻿using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Application.Catalog.Products.Dtos.Manage
{
    public class ProductCreateRequest
    {

        public string Name { set; get; }
        public decimal Price { set; get; }
        public decimal OrOriginalPrice { set; get; }
        public int Stock { set; get; }
        public string DesDescription { set; get; }
        public string Details { set; get; }
        public string SeoDescription { get; set; }
        public string ThumbnailImage { get; set; }
        public string LanguageId { get; set; }
        public string SeoTitle { get; set; }
        public string SeoAlias { get; set; }
    }

}
