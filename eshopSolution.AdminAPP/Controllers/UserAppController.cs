using eshopSolution.AdminAPP.Service;
using eShopSolution.ViewModels.Common;
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
        private readonly IRoleApiClient _iroleApiClient;

        public UserAppController(IUserApiClient iuserApiClient, IRoleApiClient iroleApiClient)
        {
            _iuserApiClient = iuserApiClient;
            _iroleApiClient = iroleApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageindex = 1, int pagsize = 10)
        {
            var session = HttpContext.Session.GetString("Token");
            var request = new GetUserPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageindex,
                PageSize = pagsize,
            };
            var data = await _iuserApiClient.Listuser(request);
            ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }

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
            {
                TempData["result"] = "Update Successfully!";
                return RedirectToAction("Index");
            }

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
            {
                TempData["result"] = "Delete successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _iuserApiClient.UserbyId(id);
            return View(result.ResultObj);
        }

        [HttpGet]
        public async Task<IActionResult> SuggestSearch()
        {
            string keyword = HttpContext.Request.Query["term"].ToString();
            var result = await _iuserApiClient.SuggestSearch(keyword);
            if (result.IsSuccessed)
            {
                return Ok(result.ResultObj);
            }
            return Ok(result.ResultObj);
        }

        [HttpGet]
        public async Task<IActionResult> RolesAssign(Guid Id)
        {
            var roleAssignRequst = await GetRoleAssignRequest(Id);
            return View(roleAssignRequst);
        }

        [HttpPost]
        public async Task<IActionResult> RolesAssign(RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return View();
            var result = await _iuserApiClient.RoleAssign(request.Id, request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Assign Successfully!";
                return RedirectToAction("Index", "UserApp");
            }
            ModelState.AddModelError("", result.Message);
            var roleAssignRequst = await GetRoleAssignRequest(request.Id);
            return View(roleAssignRequst);
        }

        private async Task<RoleAssignRequest> GetRoleAssignRequest(Guid Id)
        {
            var userObj = await _iuserApiClient.UserbyId(Id);
            var roleObj = await _iroleApiClient.GetAll();
            var roleAssignRequest = new RoleAssignRequest();
            foreach (var role in roleObj.ResultObj)
            {
                roleAssignRequest.Roles.Add(new SelectItem()
                {
                    Id = role.Id.ToString(),
                    Name = role.Name,
                    Selected = userObj.ResultObj.Roles.Contains(role.Name),
                });
            }
            return roleAssignRequest;
        }
    }
}