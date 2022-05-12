using eShopSolution.Application.System.Users;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolutionBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class usersController : Controller
    {
        private readonly IUserService _iuserservice;
        private readonly IConfiguration _configuration;

        public usersController(IUserService iuserService, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _iuserservice = iuserService;
            _configuration = configuration;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultToken = await _iuserservice.Authencate(request);
            if (string.IsNullOrEmpty(resultToken.ResultObj))
            {
                return BadRequest(resultToken);
            }
            return Ok(resultToken);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultRegister = await _iuserservice.Register(request);
            return Ok(resultRegister);
        }

        [HttpGet("listuser")]
        public async Task<IActionResult> listuser([FromQuery] GetUserPagingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var ListUser = await _iuserservice.GetUserPaging(request);
            return Ok(ListUser);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ResultUpdate = await _iuserservice.UpdateUser(id, request);
            return Ok(ResultUpdate);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Userbyid(Guid id)
        {
            var result = await _iuserservice.GetUserId(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _iuserservice.DeleteUser(id);
            return Ok(result);
        }

        [HttpGet("suggestsearch/{keyword}")]
        public async Task<IActionResult> SuggestSearch(string keyword)
        {
            var result = await _iuserservice.SuggestSearch(keyword);
            return Ok(result);
        }

        [HttpPut("rolesassign/{Id}/roles")]
        public async Task<IActionResult> RoleAssign(Guid Id, [FromBody] RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _iuserservice.RoleAssign(Id, request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
    }
}