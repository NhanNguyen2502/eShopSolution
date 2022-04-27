using eShopSolution.ViewModels.System.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Service
{
    public class UserApiClient : IUserApiClient
    {
        private readonly IHttpClientFactory _ihttpClientfactory;

        public UserApiClient(IHttpClientFactory ihttpClientfactory
      )
        {
            _ihttpClientfactory = ihttpClientfactory;
        }

        public async Task<string> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _ihttpClientfactory.CreateClient();
            client.BaseAddress = new Uri("http://nhan12588-001-site1.ctempurl.com");
            var reponse = await client.PostAsync("/api/users/authenticate", httpcontent);
            var token = await reponse.Content.ReadAsStringAsync();
            return token;
            //http:nhan12588-001-site1.ctempurl.com
        }

        public async Task<string> Register(RegisterRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _ihttpClientfactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:5001");
            var reponse = await client.PostAsync("/api/users/register", httpcontent);
            var result = await reponse.Content.ReadAsStringAsync();
            return result;
        }
    }
}