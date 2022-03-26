using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Product
{
    public class ProductImageViewModel
    {
        public string Caption { get; set; }
        public DateTime DateCreate { get; set; }
        public int ProductId { get; set; }
        public int Id { set; get; }
        public string FilePath { set; get; }
        public bool IsDefault { set; get; }
        public int SortOrder { get; set; }
        public long FileSize { set; get; }
    }
}
