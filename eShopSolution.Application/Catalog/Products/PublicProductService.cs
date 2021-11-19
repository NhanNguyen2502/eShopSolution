using eShopSolution.Application.Catalog.Dtos;
using eShopSolution.Application.Catalog.Products.Dtos.Public;
using eShopSolution.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace eShopSolution.Application.Catalog.Products
{
    class PublicProductService : IPublicProductService
    {
        private readonly EShopDbContext _Context;
        public PublicProductService(EShopDbContext Context)
        {
            _Context = Context;
        }
        public async Task<PagedResult<ProductViewModel>> GetAllCategory(GetProductPagingRequest request)
        {
            // Step1: Select Join 
            var query = from p in _Context.Products
                        join pt in _Context.ProductTranslations on p.ID equals pt.ProductId
                        join pic in _Context.ProductInCategories on p.ID equals pic.ProductId
                        join c in _Context.Categories on pic.CategoryId equals c.ID
                        select new { p, pt, pic };
            //Step2: Filter
            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);
            }
            //Step3: Paging
            int TotalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductViewModel()
                {
                    ID = x.p.ID,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,

                }).ToListAsync();
            //Step4: Select and Projection
            var pageResult = new PagedResult<ProductViewModel>()
            {
                TotalRecord = TotalRow,
                Items = data,
            };
            return pageResult;
        }
    }
}
