using eShopSolutio.Utilities.Exceptions;
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

        public async Task<APIResultMessage<bool>> Create(ProductCreateRequest request)
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

                return new ApiSuccessResult<bool>();
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
            }
        }

        public async Task<APIResultMessage<bool>> Delete(int ProductId)
        {
            try
            {
                var product = await _Context.Products.FindAsync(ProductId);
                if (product == null)
                {
                    return new ApiErrorResult<bool>("Product dose not exist!");
                }
                var images = _Context.ProductImages.Where(i => i.ProductId == ProductId);
                foreach (var image in images)
                {
                    await _storageService.DeleteFileAsync(image.ImagePath);
                }
                _Context.Products.Remove(product);
                await _Context.SaveChangesAsync();
                return new ApiSuccessResult<bool>();
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
            }
        }

        public async Task<APIResultMessage<PagedResult<ProductViewModel>>> GetAllPaging(GetManageProductPagingRequest request)
        {
            try
            { // Step1: Select Join
                var query = from p in _Context.Products
                            join pt in _Context.ProductTranslations on p.ID equals pt.ProductId
                            join pi in _Context.ProductImages on p.ID equals pi.ProductId into pii
                            from pi in pii.DefaultIfEmpty()
                            join pic in _Context.ProductInCategories on p.ID equals pic.ProductId into piic
                            from pic in piic.DefaultIfEmpty()
                            join c in _Context.Categories on pic.CategoryId equals c.ID into cic
                            from c in cic.DefaultIfEmpty()
                            where pt.LanguageId == request.LanguageId && pi.IsDefault == true
                            select new { p, pi, pic, pt, c };

                //var query = (from p in _Context.Products
                //             from pt in _Context.ProductTranslations
                //             .Where(x => x.ProductId == p.ID && x.LanguageId == request.LanguageId)
                //             from pi in _Context.ProductImages
                //             .Where(x => x.ProductId == p.ID)
                //             .DefaultIfEmpty()
                //             from pc in _Context.ProductInCategories
                //             .Where(x => x.ProductId == p.ID)
                //             .DefaultIfEmpty()
                //             select new
                //             {
                //                 ProductId = p.ID,
                //                 DateCreated = p.DateCreated,
                //                 Price = p.Price,
                //                 OriginalPrice = p.OriginalPrice,
                //                 Stock = p.Stock,
                //                 Name = pt.Name,
                //                 ImgLink = pi.ImagePath,
                //                 CategorId = pc.CategoryId,
                //             });

                var isCheck = string.IsNullOrEmpty(request.Keyword);
                //Step2: Filter
                if (!isCheck)
                    query = query.Where(x => x.pt.Name.Contains(request.Keyword));
                if (request.CategoryId != null && request.CategoryId != 0)
                {
                    query = query.Where(x => x.pic.CategoryId == request.CategoryId);
                }
                //Step3: Paging
                int TotalRow = await query.CountAsync();
                var products = await query.Select(x => new ProductViewModel()
                {
                    Id = x.p.ID,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    Stock = x.p.Stock,
                    ImgLink = x.pi.ImagePath != null ? _storageService.GetFileUrl(x.pi.ImagePath) : _storageService.GetFileUrl("faf40bd2-7085-49cd-ae33-234fa2980aba.png"), //x.pi.ImagePath,
                }).ToListAsync();
                var productsort = products.OrderByDescending(x => x.DateCreated);
                var data = productsort.Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize).ToList();

                //Step4: Select and Projection
                var pageResult = new PagedResult<ProductViewModel>()
                {
                    pageIndex = request.PageIndex,
                    pagSize = request.PageSize,
                    TotalRecord = TotalRow,
                    Items = data,
                };
                return new ApiSuccessResult<PagedResult<ProductViewModel>>(pageResult);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<PagedResult<ProductViewModel>>(ex.Message);
            }
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

        public async Task<APIResultMessage<ProductDetailVM>> GetProductId(int ProductID, string LanguageId)
        {
            var product = await _Context.Products.FindAsync(ProductID);
            //var catagory = await _Context.ProductInCategories.FirstOrDefaultAsync(x => x.ProductId == product.ID);
            var productranlation = await _Context.ProductTranslations.FirstOrDefaultAsync(a => a.ProductId == product.ID &&
            a.LanguageId == LanguageId);
            var categories = await (from c in _Context.Categories
                                    join ct in _Context.CategoryTranslations on c.ID equals ct.CategoryId
                                    join pic in _Context.ProductInCategories on c.ID equals pic.CategoryId
                                    where ct.LanguageId == LanguageId && pic.ProductId == ProductID
                                    select ct.Name).ToListAsync();

            var productdetail = new ProductDetailVM()
            {
                Id = product.ID,
                Name = productranlation.Name != null ? productranlation.Name : null,
                Price = product.Price,
                OrOriginalPrice = product.OriginalPrice,
                DateCreated = product.DateCreated,
                Stock = product.Stock,
                //CategoryId = catagory.CategoryId != null ? catagory.CategoryId : null,
                SeoTitle = productranlation.SeoTitle,
                DesDescription = productranlation.Description != null ? productranlation.Description : null,
                LanguageId = productranlation.LanguageId != null ? productranlation.LanguageId : null,
                Categories = categories
            };
            return new ApiSuccessResult<ProductDetailVM>(productdetail);
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

        public async Task<APIResultMessage<List<string>>> Search(string keywork, string LanguageId)
        {
            try
            {
                var ProductName = await _Context.ProductTranslations.Where(p => p.Name.Contains(keywork)).Take(5)
                    .Where(x => x.LanguageId == LanguageId).Select(p => p.Name).ToListAsync();
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

        public async Task<APIResultMessage<bool>> Update(ProductUpdateRequest request)
        {
            try
            {
                var product = await _Context.Products.FindAsync(request.Id);
                var productTranlations = await _Context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == request.Id);

                if (product == null || productTranlations == null) throw new EShopException($"Cannot find a product with id: {request.Id}");
                product.Price = request.NewPrice;
                product.OriginalPrice = request.NeworOriginalPrice;
                product.Stock = request.Stock;
                productTranlations.Name = request.Name;
                productTranlations.SeoTitle = request.SeoTitle;
                productTranlations.Description = request.Description;
                //Update  Image
                //if (request.ThumbnailImage != null)
                //{
                //    var thumbnailImage = await _Context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == request.Id);
                //    if (thumbnailImage != null)
                //    {
                //        thumbnailImage.FileSize = request.ThumbnailImage.Length;
                //        thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                //        _Context.ProductImages.Update(thumbnailImage);
                //    }
                //}
                _Context.Products.Update(product);
                _Context.ProductTranslations.Update(productTranlations);
                await _Context.SaveChangesAsync();
                return new ApiSuccessResult<bool>();
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
            }
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

        public async Task<APIResultMessage<bool>> UpdatePrice(ProductUpdateRequest request)
        {
            try
            {
                var product = await _Context.Products.FindAsync(request.Id);
                if (product == null) //throw new EShopException($"Cannot find a product with id: {productId}");
                    return new ApiErrorResult<bool>("Product dose not exist!");
                product.Price = request.NewPrice;
                _Context.Products.Update(product);
                await _Context.SaveChangesAsync();
                return new ApiSuccessResult<bool>();
            }
            catch (Exception ex) { return new ApiErrorResult<bool>(ex.Message); }
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
                        Id = x.p.ID,
                        Name = x.pt.Name,
                        DateCreated = x.p.DateCreated,
                        OriginalPrice = x.p.OriginalPrice,
                        Price = x.p.Price,
                        Stock = x.p.Stock,
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

        public async Task<APIResultMessage<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            var product = await _Context.Products.FindAsync(id);
            if (product == null)
            {
                return new ApiErrorResult<bool>("Product dose not exist.");
            }
            foreach (var category in request.Categories)
            {
                var productIncategory = _Context.ProductInCategories.FirstOrDefault(x => x.CategoryId == int.Parse(category.Id)
               && x.ProductId == id);
                if (productIncategory != null && category.Selected == false)
                {
                    _Context.ProductInCategories.Remove(productIncategory);
                }
                else if (productIncategory == null && category.Selected)
                {
                    await _Context.ProductInCategories.AddAsync(new ProductInCategory()
                    {
                        ProductId = id,
                        CategoryId = int.Parse(category.Id),
                    });
                }
            }
            await _Context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }
    }
}