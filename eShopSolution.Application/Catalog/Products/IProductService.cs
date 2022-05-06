using eShopSolution.ViewModels.Catalog.Product;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Result;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Result;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IProductService
    {
        Task<ResultModel> Create(ProductCreateRequest request);

        Task<ResultModel> GetProductId(int ProductID, string LanguageId);

        Task<int> Update(ProductUpdateRequest request);

        Task<ResultModel> Delete(int ProductId);

        Task<bool> UpdatePrice(int productId, decimal newPrice);

        Task<bool> UpdateStock(int productId, int addQuality);

        Task<bool> AddViewCount(int ProductId);

        Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request);

        Task<int> AddIamges(int productid, ProductImageCreateRequest request);

        Task<ResultModel> RemoveImages(int imageId);

        Task<int> UpdateImage(int imageID, int productid, ProductImageUpdateRequest request);

        Task<List<ProductImageViewModel>> GetListImage(int productId);

        Task<ResultModel> GetAllCategory(string languageid, GetPublicProductPagingRequest request);

        Task<ResultModel> Search(string term);
    }
}