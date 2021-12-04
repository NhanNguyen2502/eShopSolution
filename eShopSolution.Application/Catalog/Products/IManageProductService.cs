using eShopSolution.ViewModels.Catalog.Product;
using eShopSolution.ViewModels.Catalog.Product.Manage;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request);
        Task<int> Update(ProductUpdateRequest request);
        Task<int> Delete(int ProductId);
        Task<bool> UpdatePrice(int productId, decimal newPrice);
        Task<bool> UpdateStock(int productId, int addQuality);
        Task<bool> AddViewCount(int ProductId);
        Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request);

 

    }
}
