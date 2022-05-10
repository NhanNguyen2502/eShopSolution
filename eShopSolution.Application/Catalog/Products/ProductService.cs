﻿using eShopSolutio.Utilities.Exceptions;
using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Catalog.Product;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Result;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Result;
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
    public class ProductService : IProductService
    {
        private readonly EShopDbContext _Context;
        private readonly IStorageService _storageService;

        public ProductService(EShopDbContext Context, IStorageService storageService)
        {
            _Context = Context;
            _storageService = storageService;
        }

        public async Task<int> AddIamges(int productId, ProductImageCreateRequest request)
        {
            var listisdefault = await _Context.ProductImages.Where(x => x.ProductId == productId && x.IsDefault).ToListAsync();
            if (listisdefault.Count() == 1 && request.IsDefault)
                throw new EShopException($"Can't have two avatars for the same product {productId}");
            var productImage = new ProductImage()
            {
                ProductId = productId,
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                SortOrder = request.SortOrder,
            };
            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _Context.ProductImages.Add(productImage);
            await _Context.SaveChangesAsync();
            return productImage.Id; throw new EShopException("Add successfully!");
        }

        public async Task<bool> AddViewCount(int ProductId)
        {
            var product = await _Context.Products.FindAsync(ProductId);
            product.ViewCount += 1;
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<ResultModel> Create(ProductCreateRequest request)
        {
            try
            {
                var product = new Product()
                {
                    ProductName = request.Name,
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
                product.ProductInCategories = new List<ProductInCategory>
            {
                new ProductInCategory()
                {
                   CategoryId = request.CategoryId,
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
                await _Context.SaveChangesAsync();
                return new ResultModel()
                {
                    Message = "Creted Successfully!",
                    Result = true,
                    Data = product.ID
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResultModel> Delete(int ProductId)
        {
            try
            {
                var product = await _Context.Products.FindAsync(ProductId);
                if (product == null) //throw new EShopException($"Cannot find a product: {ProductId}");
                {
                    return new ResultModel()
                    {
                        Result = false,
                        Message = "Cannot find a product: {ProductId}"
                    };
                }
                var images = _Context.ProductImages.Where(i => i.ProductId == ProductId);
                foreach (var image in images)
                {
                    await _storageService.DeleteFileAsync(image.ImagePath);
                }
                _Context.Products.Remove(product);
                return new ResultModel()
                {
                    Message = "Delete Successfully!",
                    Result = true
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    Message = ex.Message,
                };
            }
        }

        public async Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request)
        {
            // Step1: Select Join
            var query = from p in _Context.Products
                        join pt in _Context.ProductTranslations on p.ID equals pt.ProductId
                        join pic in _Context.ProductInCategories on p.ID equals pic.ProductId
                        join c in _Context.Categories on pic.CategoryId equals c.ID
                        select new { p, pt, pic };

            var isCheck = string.IsNullOrEmpty(request.Keyword);
            //Step2: Filter
            if (!isCheck)
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

        public async Task<List<ProductImageViewModel>> GetListImage(int productId)
        {
            return await _Context.ProductImages.Where(x => x.ProductId == productId)
                .Select(i => new ProductImageViewModel()
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Caption = i.Caption,
                    FilePath = i.ImagePath,
                    FileSize = i.FileSize,
                    IsDefault = i.IsDefault,
                    SortOrder = i.SortOrder,
                    DateCreate = i.DateCreated,
                }).ToListAsync();
        }

        public async Task<ResultModel> GetProductId(int ProductID, string LanguageId)
        {
            try
            {
                var product = await _Context.Products.FindAsync(ProductID);
                var productranlation = await _Context.ProductTranslations.FirstOrDefaultAsync(a => a.ProductId == product.ID &&
                a.LanguageId == LanguageId);
                //var language = await _Context.Languages.FirstOrDefaultAsync(x => x.Id == productranlation.LanguageId);
                var Productviewmodel = new ProductViewModel()
                {
                    ID = product.ID,
                    Name = productranlation != null ? productranlation.Name : null,
                    Price = product.Price,
                    OriginalPrice = product.OriginalPrice,
                    DateCreated = product.DateCreated,
                    Stock = product.Stock,
                    ViewCount = product.ViewCount,
                    Description = productranlation != null ? productranlation.Description : null,
                    LanguageId = productranlation != null ? productranlation.LanguageId : null,
                };
                if (product == null || productranlation == null)
                {
                    return new ResultModel
                    {
                        Message = "Can't found data!",
                        Result = false,
                    };
                }
                return new ResultModel
                {
                    Data = Productviewmodel,
                    Message = "Get Product Successfully!",
                    Result = true,
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResultModel> RemoveImages(int imageId)
        {
            try
            {
                var ProductImage = await _Context.ProductImages.FindAsync(imageId);
                if (ProductImage == null) //throw new EShopException($"Cannot Find an image with id {imageId}");
                {
                    return new ResultModel()
                    {
                        Message = $"Cannot Find an image with id {imageId}",
                        Result = false
                    };
                }
                await _storageService.DeleteFileAsync(ProductImage.ImagePath);
                _Context.ProductImages.Remove(ProductImage);
                await _Context.SaveChangesAsync();
                return new ResultModel()
                {
                    Message = "Delete Image Successfully!",
                    Result = true
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    Message = ex.Message,
                    Result = false
                };
            }
        }

        public async Task<APIResultMessage<List<string>>> Search(string term)
        {
            try
            {
                var ProductName = await _Context.ProductTranslations.Where(p => p.Name.Contains(term)).Select(p => p.Name).ToListAsync();
                if (ProductName.Count() > 0)
                {
                    return new ApiSuccessResult<List<string>>(ProductName);
                }
                return new ApiErrorResult<List<string>>("ProductName not found!");
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<List<string>>(ex.Message);
            }
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
            //Update  Image
            if (request.ThumbnailImage != null)
            {
                var thumbnailImage = await _Context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == request.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    _Context.ProductImages.Update(thumbnailImage);
                }
            }
            return await _Context.SaveChangesAsync();
        }

        public async Task<int> UpdateImage(int imageId, int productid, ProductImageUpdateRequest request)
        {
            var productImage = await _Context.ProductImages.FindAsync(imageId);
            if (productImage == null) throw new EShopException($"Cannot Find an image with id {imageId}");
            var isdefault = await _Context.ProductImages.Where(x => x.ProductId == productid && x.IsDefault && x.Id != imageId).ToListAsync();
            if (isdefault.Count() >= 1) throw new EShopException($"Can't have two avatars for the same product {productid}");
            productImage.Caption = request.Caption;
            productImage.IsDefault = request.IsDefault;
            productImage.SortOrder = request.SortOrder;
            productImage.DateCreated = DateTime.Now;
            if (productImage.ImagePath != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _Context.ProductImages.Update(productImage);
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

        public async Task<ResultModel> GetAllCategory(string languageid, GetPublicProductPagingRequest request)
        {
            // Step1: Select Join
            try
            {
                var query = from p in _Context.Products
                            join pt in _Context.ProductTranslations on p.ID equals pt.ProductId
                            join pic in _Context.ProductInCategories on p.ID equals pic.ProductId
                            join c in _Context.Categories on pic.CategoryId equals c.ID
                            where pt.LanguageId == languageid
                            select new { p, pt, pic };
                //Step2: Filter
                if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
                {
                    query = query.Where(p => p.pic.CategoryId == request.CategoryId);
                }
                //Step3: Paging
                int TotalRow = await query.CountAsync();
                if (TotalRow != 0)
                {
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
                        pagSize = request.PageSize,
                        pageIndex = request.PageIndex,
                    };
                    return
                    new ResultModel
                    {
                        Result = true,
                        Message = "Get list product successfully!",
                        Data = pageResult
                    };
                }
                return new ResultModel()
                {
                    Result = false,
                    Message = "Can't found data!",
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }
    }
}