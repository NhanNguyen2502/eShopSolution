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
    public class LoginAppController : Controller
    {
        private readonly IUserApiClient _iuserApiClient;
        private readonly IConfiguration _configuration;

        public LoginAppController(IUserApiClient iuserApiClient, IConfiguration configuration)
        {
            _iuserApiClient = iuserApiClient;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return View(ModelState);
            var token = await _iuserApiClient.Authenticate(request);
            if (!token.IsSuccessed)
            {
                ModelState.AddModelError("", token.Message);
                return View();
            }
            var userprincical = this.ValidateToken((string)token.ResultObj);
            var authproperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true
            };
            HttpContext.Session.SetString("Token", token.ResultObj);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userprincical,
                authproperties);
            return RedirectToAction("Index", "Home");
        }

        private ClaimsPrincipal ValidateToken(string JwtToken)
        {
            IdentityModelEventSource.ShowPII = true;
            SecurityToken Issuer;
            TokenValidationParameters validationParameters = new TokenValidationParameters();
            validationParameters.ValidateLifetime = true;
            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            byte[] signingKeyBytes = Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]);
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes);
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(JwtToken, validationParameters, out Issuer);
            return principal;
        }
    }
}