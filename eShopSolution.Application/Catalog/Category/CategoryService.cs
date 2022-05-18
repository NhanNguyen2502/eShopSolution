using eShopSolution.Data.EF;
using eShopSolution.ViewModels.Catalog.Category;
using eShopSolution.ViewModels.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace eShopSolution.Application.Catalog.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly EShopDbContext _Context;

        public CategoryService(EShopDbContext Context)
        {
            _Context = Context;
        }

        public async Task<APIResultMessage<List<CategoryVm>>> GetAll(string LanguageId)
        {
            var query = from c in _Context.Categories
                        join ct in _Context.CategoryTranslations on c.ID equals ct.CategoryId
                        where ct.LanguageId == LanguageId
                        select new { c, ct };
            var data = await query.Select(x => new CategoryVm()
            {
                Id = x.c.ID,
                Name = x.ct.Name,
            }).ToListAsync();
            return new ApiSuccessResult<List<CategoryVm>>(data);
        }
    }
}