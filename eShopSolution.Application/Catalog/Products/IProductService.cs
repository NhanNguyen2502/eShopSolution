using eShopSolution.ViewModels.Catalog.Category;
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
        Task<APIResultMessage<bool>> Create(ProductCreateRequest request);

        Task<APIResultMessage<ProductDetailVM>> GetProductId(int ProductID, string LanguageId);

        Task<APIResultMessage<bool>> Update(ProductUpdateRequest request);

        Task<APIResultMessage<bool>> Delete(int ProductId);

        public Task<APIResultMessage<bool>> UpdatePrice(ProductUpdateRequest request);

        Task<bool> UpdateStock(int productId, int addQuality);

        Task<bool> AddViewCount(int ProductId);

        Task<APIResultMessage<PagedResult<ProductViewModel>>> GetAllPaging(GetManageProductPagingRequest request);

        Task<int> AddIamges(int productid, ProductImageCreateRequest request);

        Task<ResultModel> RemoveImages(int imageId);

        Task<int> UpdateImage(int imageID, int productid, ProductImageUpdateRequest request);

        Task<List<ProductImageViewModel>> GetListImage(int productId);

        Task<ResultModel> GetAllCategory(string languageid, GetPublicProductPagingRequest request);

        Task<APIResultMessage<List<string>>> Search(string keywork, string LanguageId);

        Task<APIResultMessage<bool>> CategoryAssign(int id, CategoryAssignRequest request);
    }
}