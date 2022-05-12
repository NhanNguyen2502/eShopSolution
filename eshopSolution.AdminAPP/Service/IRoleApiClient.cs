using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Service
{
    public interface IRoleApiClient
    {
        public Task<APIResultMessage<List<RoleVM>>> GetAll();
    }
}