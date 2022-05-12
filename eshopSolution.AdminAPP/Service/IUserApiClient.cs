using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Service
{
    public interface IUserApiClient
    {
        public Task<APIResultMessage<string>> Authenticate(LoginRequest request);

        public Task<APIResultMessage<PagedResult<UserVM>>> Listuser(GetUserPagingRequest request);

        public Task<APIResultMessage<bool>> register(RegisterRequest request);

        public Task<APIResultMessage<bool>> UpdateUser(Guid id, UserUpdateRequest request);

        public Task<APIResultMessage<UserVM>> UserbyId(Guid id);

        public Task<APIResultMessage<bool>> DeleteUser(Guid id);

        public Task<APIResultMessage<List<string>>> SuggestSearch(string keywork);

        public Task<APIResultMessage<bool>> RoleAssign(Guid Id, RoleAssignRequest request);
    }
}