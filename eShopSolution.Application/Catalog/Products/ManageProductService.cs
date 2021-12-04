using eShopSolutio.Utilities.Exceptions;
using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Catalog.Product;
using eShopSolution.ViewModels.Catalog.Product.Manage;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    class ManageProductService : IManageProductService
    {
        private readonly EShopDbContext _Context;
        private readonly IStorageService _storageService;
        public ManageProductService(EShopDbContext Context, IStorageService storageService)
        {
            _Context = Context;
            _storageService = storageService;
        }


        public async Task<bool> AddViewCount(int ProductId)
        {
            var product = await _Context.Products.FindAsync(ProductId);
            product.ViewCount += 1;
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OrOriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>
                {
                    new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.DesDescription,
                        Details = request.Details,
                        SeoDescription = request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId
                    }
                }
            };
            //Save Image
            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>
                {
                    new ProductImage()
                    {
                        Caption = "Thumnail Images",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await  this.SaveFile(request.ThumbnailImage),
                        IsDefault =true,
                        SortOrder=1
                    }
                };
            }
            _Context.Products.Add(product);
            return await _Context.SaveChangesAsync();
        }

        public async Task<int> Delete(int ProductId)
        {
            var product = await _Context.Products.FindAsync(ProductId);
            if (product == null) throw new EShopException($"Cannot find a product: {ProductId}");
            _Context.Products.Remove(product);
            return await _Context.SaveChangesAsync();

        }


        public async Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request)
        {
            // Step1: Select Join 
            var query = from p in _Context.Products
                        join pt in _Context.ProductTranslations on p.ID equals pt.ProductId
                        join pic in _Context.ProductInCategories on p.ID equals pic.ProductId
                        join c in _Context.Categories on pic.CategoryId equals c.ID
                        select new { p, pt, pic };
            //Step2: Filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));
            if (request.CategoryIds.Count > 0)
            {
                query = query.Where(p => request.CategoryIds.Contains(p.pic.CategoryId));
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

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _Context.Products.FindAsync(request.Id);
            var productTranlations = await _Context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == request.Id
            && x.LanguageId == request.LanguageId);

            if (product == null || productTranlations == null) throw new EShopException($"Cannot find a product with id: {request.Id}");
            productTranlations.Name = request.Name;
            productTranlations.SeoAlias = request.SeoAlias;
            productTranlations.SeoDescription = request.SeoDescription;
            productTranlations.SeoTitle = request.SeoTitle;
            productTranlations.Description = request.Description;
            productTranlations.Details = request.Details;
            return await _Context.SaveChangesAsync();
        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _Context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find a product with id: {productId}");
            product.Price = newPrice;
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStock(int productId, int addQuality)
        {
            var product = await _Context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find a product with id: {productId}");
            product.Stock = addQuality;
            return await _Context.SaveChangesAsync() > 0;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }
    }
}
