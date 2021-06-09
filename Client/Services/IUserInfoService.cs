using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PPProject.Client.Services
{
    public interface IUserInfoService
    {
        public Task<Address> GetUserAddressAsync();
    }
}
