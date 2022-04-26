using eShopSolutio.Utilities.Exceptions;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Catalog.Result;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly EShopDbContext _context;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager
            , RoleManager<AppRole> roleManager, IConfiguration config, EShopDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _context = context;
        }

        public async Task<string> Authencate(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null) return null;
                var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
                if (!result.Succeeded)
                {
                    user.AccessFailedCount += 1;
                    //user.LockoutEnabled = false;
                    await _context.SaveChangesAsync();
                    return null;
                }
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new[]
                {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.GivenName,user.FirstName),
                new Claim(ClaimTypes.Role, string.Join(";",roles))
            };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                    _config["Tokens:Issuer"],
                    claims,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> Register(RegisterRequest request)
        {
            try
            {
                var usermail = _context.AppUsers.Where(x => x.Email == request.email).ToList();
                if (usermail.Count > 0) throw new EShopException($"Email already exist");
                var username = _context.AppUsers.Where(x => x.UserName == request.UserName).ToList();
                if (username.Count > 0) throw new EShopException($"UserName already exist");
                var user = new AppUser()
                {
                    Email = request.email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Dob = (DateTime)request.Dob,
                    UserName = request.UserName,
                    LockoutEnabled = false,
                    CreatedAt = DateTime.Now
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded) throw new EShopException($"Register Successfully");

                throw new EShopException($"Register Fail");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private bool IsvalidEmail(string email)
        {
            try
            {
                string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                if (regex.IsMatch(email))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}