using eShopSolutio.Utilities.Exceptions;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Catalog.Result;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<APIResultMessage<string>> Authencate(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null) return new ApiErrorResult<string>("Đăng nhập không đúng");
                var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
                if (!result.Succeeded)
                {
                    user.AccessFailedCount += 1;
                    //user.LockoutEnabled = false;
                    await _context.SaveChangesAsync();
                    return new ApiErrorResult<string>("Wrong username or password");
                }
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new[]
                {
                //new Claim(ClaimTypes.Email,user.Email),
                //new Claim(ClaimTypes.Name,user.UserName),
                //new Claim(ClaimTypes.GivenName,user.FirstName),
                //new Claim(ClaimTypes.Role, string.Join(";",roles))
                 new Claim(ClaimTypes.Name,user.UserName),
                 new Claim("email",user.Email),
                 new Claim("userName",user.UserName),
                 new Claim("roles", string.Join(";",roles))
                 };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                    _config["Tokens:Issuer"],
                    claims,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: creds);

                return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<string>(ex.Message);
            }
        }

        public async Task<APIResultMessage<bool>> Register(RegisterRequest request)
        {
            try
            {
                var usermail = await _userManager.FindByEmailAsync(request.email);
                if (usermail != null)
                    return new ApiErrorResult<bool>("Email already exist");

                var username = await _userManager.FindByNameAsync(request.UserName);
                if (username != null)
                    return new ApiErrorResult<bool>("UserName already exist");
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
                if (result.Succeeded) //throw new EShopException($"Register Successfully");
                {
                    var roleUser = _roleManager.FindByNameAsync("user").Result;
                    if (roleUser != null)
                    {
                        await _userManager.AddToRoleAsync(user, roleUser.Name);
                    }
                    return new ApiSuccessResult<bool>();
                }

                return new ApiErrorResult<bool>("Register Fail");
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
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

        public async Task<APIResultMessage<PagedResult<UserVM>>> GetUserPaging(GetUserPagingRequest request)
        {
            try
            {
                var query = _userManager.Users;

                //Filter
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(x => x.UserName.Contains(request.Keyword)
                    || x.Email.Contains(request.Keyword));
                }
                //Step3: Paging
                int Totalrow = await query.CountAsync();
                var user = await query.Select(x => new UserVM()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Dob = x.Dob,
                    Email = x.Email,
                    CreatedAt = x.CreatedAt
                }).ToListAsync();
                var datasort = user.OrderByDescending(i => i.CreatedAt).ToList();
                var data = datasort.Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize).ToList();

                //Step4: Select and Projection
                var Pageresult = new PagedResult<UserVM>
                {
                    TotalRecord = Totalrow,
                    Items = data,
                    pagSize = request.PageSize,
                    pageIndex = request.PageIndex,
                };
                return new ApiSuccessResult<PagedResult<UserVM>>(Pageresult);
            }
            catch (Exception ex)
            {
                throw new EShopException(ex.Message);
            }
        }

        public async Task<APIResultMessage<bool>> UpdateUser(Guid id, UserUpdateRequest request)
        {
            try
            {
                if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
                {
                    return new ApiErrorResult<bool>("Email already exists!");
                }
                var user = await _userManager.FindByIdAsync(id.ToString());

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Dob = (DateTime)request.Dob;
                user.Email = request.Email;
                user.PhoneNumber = request.PhoneNumber;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return new ApiSuccessResult<bool>();
                return new ApiErrorResult<bool>("Update Fail");
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
            }
        }

        public async Task<APIResultMessage<UserVM>> GetUserId(Guid id)
        {
            try
            {
                var query = await _userManager.FindByIdAsync(id.ToString());
                if (query == null)
                {
                    return new ApiErrorResult<UserVM>("User dose not exist!");
                }
                var roles = await _userManager.GetRolesAsync(query);
                var userVM = new UserVM()
                {
                    Id = query.Id,
                    UserName = query.UserName,
                    FirstName = query.FirstName,
                    LastName = query.LastName,
                    Email = query.Email,
                    Dob = query.Dob,
                    PhoneNumber = query.PhoneNumber,
                    CreatedAt = query.CreatedAt,
                    Roles = roles
                };
                return new ApiSuccessResult<UserVM>(userVM);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<UserVM>(ex.Message);
            }
        }

        public async Task<APIResultMessage<bool>> DeleteUser(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return new ApiErrorResult<bool>("User does not exist!");
                }
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var roleName in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return new ApiSuccessResult<bool>();
                return new ApiErrorResult<bool>("Delete failed");
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
            }
        }

        public async Task<APIResultMessage<List<string>>> SuggestSearch(string keyword)
        {
            var user = await _context.AppUsers.Where(x => x.UserName.Contains(keyword) || x.Email.Contains(keyword)).Take(5)
                .Select(x => x.UserName).ToListAsync();
            if (user.Count() > 0)
                return new ApiSuccessResult<List<string>>(user);
            return new ApiErrorResult<List<string>>("User dose not exist!");
        }

        public async Task<APIResultMessage<bool>> RoleAssign(Guid Id, RoleAssignRequest request)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Account does not exist!");
            }
            var removeRoles = request.Roles.Where(x => x.Selected == false).Select(x => x.Name)
                .ToList();
            foreach (var roleName in removeRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == true)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }
        

            var addRoles = request.Roles.Where(x => x.Selected == true).Select(x => x.Name)
                .ToList();
            foreach (var roleName in addRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == false)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }
            return new ApiSuccessResult<bool>();
        }
    }
}