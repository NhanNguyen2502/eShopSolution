using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Users;
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

        public UserApiClient(IHttpClientFactory ihttpClientfactory, IConfiguration configuration)
        {
            _ihttpClientfactory = ihttpClientfactory;
            _configuration = configuration;
        }

        public async Task<string> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _ihttpClientfactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var reponse = await client.PostAsync("/api/users/authenticate", httpcontent);
            var token = await reponse.Content.ReadAsStringAsync();
            return token;
        }

        public async Task<PagedResult<UserVM>> Listuser(GetUserPagingRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _ihttpClientfactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.BearerToken);
            var reponse = await client.GetAsync($"/api/users/listuser?Keyword=" + $"{request.Keyword}&PageIndex={ request.PageIndex}&PageSize={request.PageSize}");
            var body = await reponse.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<PagedResult<UserVM>>(body);
            return users;
        }

        public async Task<ResultMessageModel> register(RegisterRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _ihttpClientfactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var reponse = await client.PostAsync("/api/users/register", httpcontent);
            var body = await reponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultMessageModel>(body);
            return result;
        }
    }
}