using eshopSolution.AdminAPP.Service;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Controllers
{
    public class RegisterAppController : Controller
    {
        private readonly IUserApiClient _iuserApiClient;

        public RegisterAppController(IUserApiClient iuserApiClient)
        {
            _iuserApiClient = iuserApiClient;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _iuserApiClient.register(request);
                if (result.IsSuccessed == true)
                {
                    TempData["result"] = "Register Successfully!";
                    return RedirectToAction("Register");
                }
                TempData["result"] = result.Message;
                return RedirectToAction("Register");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}