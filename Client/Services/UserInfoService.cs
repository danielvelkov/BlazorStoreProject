using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PPProject.Client.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly HttpClient httpClient;
        public UserInfoService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Address> GetUserAddressAsync() =>
            await httpClient.GetFromJsonAsync<Address>("api/address");
    }
}
