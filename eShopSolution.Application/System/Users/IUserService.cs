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
        public Task<string> Authencate(LoginRequest request);

        public Task<ResultMessageModel> Register(RegisterRequest request);

        Task<PagedResult<UserVM>> GetUserPaging(GetUserPagingRequest request);
    }
}