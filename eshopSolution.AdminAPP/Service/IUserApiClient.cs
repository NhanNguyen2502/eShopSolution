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
        public Task<string> Authenticate(LoginRequest request);

        public Task<PagedResult<UserVM>> Listuser(GetUserPagingRequest request);

        public Task<ResultMessageModel> register(RegisterRequest request);
    }
}