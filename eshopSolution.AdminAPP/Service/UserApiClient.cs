using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Service
{
    public class UserApiClient : IUserApiClient
    {
        private readonly IHttpClientFactory _ihttpClientfactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpcontextaccessor;

        public UserApiClient(IHttpClientFactory ihttpClientfactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _ihttpClientfactory = ihttpClientfactory;
            _configuration = configuration;
            _httpcontextaccessor = httpContextAccessor;
        }

        public async Task<APIResultMessage<string>> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _ihttpClientfactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var reponse = await client.PostAsync("/api/users/authenticate", httpcontent);
            var body = await reponse.Content.ReadAsStringAsync();
            if (reponse.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(body);
            }
            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await reponse.Content.ReadAsStringAsync());
        }

        public async Task<APIResultMessage<bool>> DeleteUser(Guid id)
        {
            var session = _httpcontextaccessor.HttpContext.Session.GetString("Token");
            var client = _ihttpClientfactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.DeleteAsync($"/api/users/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == true)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);
            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<APIResultMessage<PagedResult<UserVM>>> Listuser(GetUserPagingRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
            var session = _httpcontextaccessor.HttpContext.Session.GetString("Token");
            var client = _ihttpClientfactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            var reponse = await client.GetAsync($"/api/users/listuser?Keyword=" + 
                $"{request.Keyword}&PageIndex={ request.PageIndex}&PageSize={request.PageSize}");
            var body = await reponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<UserVM>>>(body);
        }

        public async Task<APIResultMessage<bool>> register(RegisterRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _ihttpClientfactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var reponse = await client.PostAsync("/api/users/register", httpcontent);
            var body = await reponse.Content.ReadAsStringAsync();
            if (reponse.IsSuccessStatusCode == true)

                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<APIResultMessage<bool>> RoleAssign(Guid Id, RoleAssignRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                
                var sesion = _httpcontextaccessor.HttpContext.Session.GetString("Token");
                var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
                var client = _ihttpClientfactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sesion);
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var reponse = await client.PutAsync($"/api/users/rolesassign/{Id}/roles", httpcontent);
                var body = await reponse.Content.ReadAsStringAsync();
                if (reponse.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);
                return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
            }
        }

        public async Task<APIResultMessage<List<string>>> SuggestSearch(string keywork)
        {
            try
            {
                var sesion = _httpcontextaccessor.HttpContext.Session.GetString("Token");
                var client = _ihttpClientfactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sesion);
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var reponse = await client.GetAsync($"/api/users/suggestsearch/{keywork}");
                var body = await reponse.Content.ReadAsStringAsync();
                if (reponse.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<ApiSuccessResult<List<string>>>(body);
                return JsonConvert.DeserializeObject<ApiErrorResult<List<string>>>(body);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<List<string>>(ex.Message);
            }
        }

        public async Task<APIResultMessage<bool>> UpdateUser(Guid id, UserUpdateRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
                var client = _ihttpClientfactory.CreateClient();
                var session = _httpcontextaccessor.HttpContext.Session.GetString("Token");
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
                var reponse = await client.PutAsync($"/api/users/update/{id}", httpcontent);
                var body = await reponse.Content.ReadAsStringAsync();
                if (reponse.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);
                return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
            }
        }

        public async Task<APIResultMessage<UserVM>> UserbyId(Guid id)
        {
            try
            {
                var session = _httpcontextaccessor.HttpContext.Session.GetString("Token");
                var client = _ihttpClientfactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
                var reponse = await client.GetAsync($"/api/users/{id}");
                if (reponse.IsSuccessStatusCode == true)
                    return JsonConvert.DeserializeObject<ApiSuccessResult<UserVM>>(await reponse.Content.ReadAsStringAsync());
                return JsonConvert.DeserializeObject<ApiErrorResult<UserVM>>(await reponse.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<UserVM>(ex.Message);
            }
        }
    }
}