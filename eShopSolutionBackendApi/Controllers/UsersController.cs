using eShopSolution.Application.System.Users;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolutionBackendApi.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userservice;

        public UsersController(IUserService userService)
        {
            _userservice = userService;
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromForm] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var resultToken = await _userservice.Authencate(request);
            if (string.IsNullOrEmpty(resultToken))
            {
                return BadRequest("UserName or Password is incorrect.");
            }
            return Ok(new { token = resultToken });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultRegister = await _userservice.Register(request);
            return Ok(resultRegister);
        }
    }
}