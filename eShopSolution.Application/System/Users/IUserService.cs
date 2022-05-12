using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Users;
using System;
using System.Collections.Generic;
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

        public Task<APIResultMessage<List<string>>> SuggestSearch(string keyword);

        public Task<APIResultMessage<bool>> RoleAssign(Guid Id, RoleAssignRequest request);
    }
}