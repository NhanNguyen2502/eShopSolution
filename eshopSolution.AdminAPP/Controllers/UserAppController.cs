using eshopSolution.AdminAPP.Service;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
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

namespace eshopSolution.AdminAPP.Controllers
{
    public class UserAppController : BaseController
    {
        private readonly IUserApiClient _iuserApiClient;

        public UserAppController(IUserApiClient iuserApiClient)
        {
            _iuserApiClient = iuserApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageindex = 1, int pagsize = 3)
        {
            var session = HttpContext.Session.GetString("Token");
            var request = new GetUserPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageindex,
                PageSize = pagsize
            };
            var data = await _iuserApiClient.Listuser(request);
            ViewBag.Keyword = keyword;
            return View(data.ResultObj);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Login", "LoginApp");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _iuserApiClient.UserbyId(id);
            if (result.IsSuccessed)
            {
                var user = result.ResultObj;
                var updaterequest = new UserUpdateRequest()
                {
                    id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Dob = user.Dob,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
                return View(updaterequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _iuserApiClient.UpdateUser(request.id, request);
            if (result.IsSuccessed)
                return RedirectToAction("Index");
            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            return View(new UserDeleteRequest()
            {
                Id = id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserDeleteRequest request)
        {
            var result = await _iuserApiClient.DeleteUser(request.Id);
            if (result.IsSuccessed)
                return RedirectToAction("Index");
            ModelState.AddModelError("", result.Message);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _iuserApiClient.UserbyId(id);
            return View(result.ResultObj);
        }

        //[HttpGet]
        //public IActionResult SuggestSearch()
        //{
        //    return View();
        //}

        [HttpGet]
        public async Task<IActionResult> SuggestSearch()
        {
            string keyword = HttpContext.Request.Query["term"].ToString();
            var result = await _iuserApiClient.SuggestSearch(keyword);
            if (result.IsSuccessed)
            {
                return Ok(result.ResultObj);
            }
            //ModelState.AddModelError("", result.Message);
            return Ok(result.ResultObj);
        }
    }
}