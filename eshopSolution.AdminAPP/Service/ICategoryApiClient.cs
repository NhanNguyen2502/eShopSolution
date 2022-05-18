using eShopSolution.ViewModels.Catalog.Category;
using eShopSolution.ViewModels.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Service
{
    public interface ICategoryApiClient
    {
        public Task<APIResultMessage<List<CategoryVm>>> GetAll(string LanguageID);
    }
}