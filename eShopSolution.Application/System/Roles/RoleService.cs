using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Roles
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<APIResultMessage<List<RoleVM>>> GetAll()
        {
            try
            {
                var roles = await _roleManager.Roles.Select(x => new RoleVM()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToListAsync();
                if (roles != null)
                {
                    return new ApiSuccessResult<List<RoleVM>>(roles);
                }
                return new ApiErrorResult<List<RoleVM>>();
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<List<RoleVM>>(ex.Message);
            }
        }
    }
}