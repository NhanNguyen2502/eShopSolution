using eShopSolution.ViewModels.Catalog.Product;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using eShopSolutio.Utilities.Contains;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace eshopSolution.AdminAPP.Service
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductApiClient(IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResultMessage<bool>> Created(ProductCreateRequest request)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var languageID = _httpContextAccessor.HttpContext.Session.GetString(SystemConstains.AppSetting.DefaultLanguageIdsys);
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            client.BaseAddress = new Uri(_configuration[SystemConstains.AppSetting.BaseAddresssys]);
            var requestContent = new MultipartFormDataContent();
            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "ThumbnailImage", request.ThumbnailImage.FileName);
            }
            requestContent.Add(new StringContent(request.Name.ToString()), "Name");
            requestContent.Add(new StringContent(request.Price.ToString()), "Price");
            requestContent.Add(new StringContent(request.OrOriginalPrice.ToString()), "OrOriginalPrice");
            requestContent.Add(new StringContent(request.Stock.ToString()), "Stock");
            requestContent.Add(new StringContent(request.SeoAlias.ToString()), "SeoAlias");
            requestContent.Add(new StringContent(request.SeoTitle.ToString()), "SeoTitle");
            requestContent.Add(new StringContent(request.SeoDescription.ToString()), "SeoDescription");
            requestContent.Add(new StringContent(request.Details.ToString()), "Details");
            requestContent.Add(new StringContent(request.DesDescription.ToString()), "DesDescription");
            requestContent.Add(new StringContent(languageID), "languageID");
            var response = await client.PostAsync($"/api/products/Create", requestContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);
            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<APIResultMessage<bool>> Delete(int id)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            var reponse = await client.DeleteAsync($"/api/products/{id}");
            var body = await reponse.Content.ReadAsStringAsync();
            if (reponse.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);
            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<APIResultMessage<PagedResult<ProductViewModel>>> GetAll(GetManageProductPagingRequest request)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            var reponse = await client.GetAsync($"/api/products/getproductpaging?Keyword={request.Keyword}"
                + $"&LanguageId={request.LanguageId}&&CategoryId={request.CategoryId}"
                + $"&PageIndex={request.PageIndex}&PageSize={request.PageSize}");
            var body = await reponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<ProductViewModel>>>(body);
        }

        public async Task<APIResultMessage<ProductDetailVM>> GetProductId(int Id)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var LanguageId = _httpContextAccessor.HttpContext.Session.GetString("DefaultLanguageId");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var reponse = await client.GetAsync($"/api/products/{Id}/{LanguageId}");
            if (reponse.IsSuccessStatusCode == true)
                return JsonConvert.DeserializeObject<APIResultMessage<ProductDetailVM>>(await reponse.Content.ReadAsStringAsync());
            return JsonConvert.DeserializeObject<APIResultMessage<ProductDetailVM>>(await reponse.Content.ReadAsStringAsync());
        }

        public async Task<APIResultMessage<List<string>>> SuggestSearch(string keyword)
        {
            try
            {
                var LanguageId = _httpContextAccessor.HttpContext.Session.GetString("DefaultLanguageId");
                var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var reponse = await client.GetAsync($"/api/products/search?keyword={keyword}&LanguageId={LanguageId}");
                var body = await reponse.Content.ReadAsStringAsync();
                if (reponse.IsSuccessStatusCode == true)
                    return JsonConvert.DeserializeObject<ApiSuccessResult<List<string>>>(body);
                return JsonConvert.DeserializeObject<ApiErrorResult<List<string>>>(body);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<List<string>>(ex.Message);
            }
        }

        public async Task<APIResultMessage<bool>> Update(ProductUpdateRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
                var session = _httpContextAccessor.HttpContext.Session.GetString(SystemConstains.AppSetting.Tokensys);
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
                client.BaseAddress = new Uri(_configuration[SystemConstains.AppSetting.BaseAddresssys]);
                var reponse = await client.PutAsync($"/api/products/Update", httpcontent);
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
    }
}