using eShopSolution.ViewModels.Catalog.Category;
using eShopSolution.ViewModels.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Category
{
    public interface ICategoryService
    {
        public Task<APIResultMessage<List<CategoryVm>>> GetAll(string LanguageId);
    }
}