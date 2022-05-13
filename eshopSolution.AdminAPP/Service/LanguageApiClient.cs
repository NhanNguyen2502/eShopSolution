using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Languages;
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
    public class LanguageApiClient : ILanguageApiClient

    {
        private readonly IHttpClientFactory _ihttpClientFactory;
        private readonly IHttpContextAccessor _ihttpContextAccessor;
        private readonly IConfiguration _iconfiguration;

        public LanguageApiClient(IHttpClientFactory ihttpClientFactory, IHttpContextAccessor ihttpContextAccessor,
            IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            _ihttpClientFactory = ihttpClientFactory;
            _ihttpContextAccessor = ihttpContextAccessor;
        }

        public async Task<APIResultMessage<List<LanguageVM>>> GetAll()
        {
            try
            {
                var session = _ihttpContextAccessor.HttpContext.Session.GetString("Token");
                var client = _ihttpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
                client.BaseAddress = new Uri(_iconfiguration["BaseAddress"]);
                var reponse = await client.GetAsync($"/api/Languages/getall");
                var body = await reponse.Content.ReadAsStringAsync();
                if (reponse.IsSuccessStatusCode)
                {
                    //List<LanguageVM> myDeserializedObjList = (List<LanguageVM>)JsonConvert.DeserializeObject(body, typeof(List<LanguageVM>));
                    //return new ApiSuccessResult<List<LanguageVM>>(myDeserializedObjList);
                    return JsonConvert.DeserializeObject<ApiSuccessResult<List<LanguageVM>>>(body);
                }
                return JsonConvert.DeserializeObject<ApiErrorResult<List<LanguageVM>>>(body);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<List<LanguageVM>>(ex.Message);
            }
        }
    }
}