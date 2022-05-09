using eShopSolution.ViewModels.Catalog.Result;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Users
{
    public interface IUserService
    {
        public Task<APIResultMessage<string>> Authencate(LoginRequest request);

        public Task<APIResultMessage<bool>> Register(RegisterRequest request);

        public Task<APIResultMessage<PagedResult<UserVM>>> GetUserPaging(GetUserPagingRequest request);

        public Task<APIResultMessage<bool>> UpdateUser(Guid id, UserUpdateRequest request);

        public Task<APIResultMessage<UserVM>> GetUserId(Guid id);

        public Task<APIResultMessage<bool>> DeleteUser(Guid id);

        public Task<APIResultMessage<object>> SuggestSearch(string keyword);
    }
}