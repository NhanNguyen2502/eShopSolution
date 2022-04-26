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

        public Task<string> Register(RegisterRequest request);
    }
}