using eShopSolution.ViewModels.Catalog.Product;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Service
{
    public interface IProductApiClient
    {
        public Task<APIResultMessage<PagedResult<ProductViewModel>>> GetAll(GetManageProductPagingRequest request);

        public Task<APIResultMessage<List<string>>> SuggestSearch(string keyword);

        public Task<APIResultMessage<ProductDetailVM>> GetProductId(int id);

        public Task<APIResultMessage<bool>> Created(ProductCreateRequest request);

        public Task<APIResultMessage<bool>> Update(ProductUpdateRequest request);

        public Task<APIResultMessage<bool>> Delete(int id);

        public Task<APIResultMessage<bool>> CategoryAssign(int id, CategoryAssignRequest request);
    }
}