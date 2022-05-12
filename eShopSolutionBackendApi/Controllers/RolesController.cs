using eShopSolution.Application.System.Roles;
using eShopSolution.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolutionBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _iroleService;

        public RolesController(IRoleService iroleService)
        {
            _iroleService = iroleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _iroleService.GetAll();
            return Ok(roles);
        }
    }
}