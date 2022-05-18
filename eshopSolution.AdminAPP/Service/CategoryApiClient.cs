using eShopSolutio.Utilities.Contains;
using eShopSolution.ViewModels.Catalog.Category;
using eShopSolution.ViewModels.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Service
{
    public class CategoryApiClient : ICategoryApiClient
    {
        private readonly IHttpClientFactory _httpClientfactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpcontextaccessor;

        public CategoryApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientfactory = httpClientFactory;
            _httpcontextaccessor = httpContextAccessor;
        }

        public async Task<APIResultMessage<List<CategoryVm>>> GetAll(string LanguageID)
        {
            try
            {
                var session = _httpcontextaccessor.HttpContext.Session.GetString(SystemConstains.AppSetting.Tokensys);
                var client = _httpClientfactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
                client.BaseAddress = new Uri(_configuration[SystemConstains.AppSetting.BaseAddresssys]);
                var reponse = await client.GetAsync($"/api/Category?LanguageID={LanguageID}");
                var body = await reponse.Content.ReadAsStringAsync();
                if (reponse.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<ApiSuccessResult<List<CategoryVm>>>(body);
                return JsonConvert.DeserializeObject<ApiErrorResult<List<CategoryVm>>>(body);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<List<CategoryVm>>(ex.Message);
            }
        }
    }
}