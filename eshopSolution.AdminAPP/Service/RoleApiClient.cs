using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Roles;
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
    public class RoleApiClient : IRoleApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _ihttpClientFactory;
        private readonly IConfiguration _iconfiguration;

        public RoleApiClient(IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory ihttpClientFactory, IConfiguration iconfiguration)
        {
            _httpContextAccessor = httpContextAccessor;
            _ihttpClientFactory = ihttpClientFactory;
            _iconfiguration = iconfiguration;
        }

        public async Task<APIResultMessage<List<RoleVM>>> GetAll()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext.Session.GetString("Token");
                var client = _ihttpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
                client.BaseAddress = new Uri(_iconfiguration["BaseAddress"]);
                var reponse = await client.GetAsync($"/api/Roles");
                var body = await reponse.Content.ReadAsStringAsync();
                if (reponse.IsSuccessStatusCode)
                {
                    //List<RoleVM> myDeserializedObjList = (List<RoleVM>)JsonConvert.DeserializeObject(body, typeof(List<RoleVM>));
                    //return new ApiSuccessResult<List<RoleVM>>(myDeserializedObjList);
                    return JsonConvert.DeserializeObject<ApiSuccessResult<List<RoleVM>>>(body);
                }

                return JsonConvert.DeserializeObject<ApiErrorResult<List<RoleVM>>>(body);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<List<RoleVM>>(ex.Message);
            }
        }
    }
}