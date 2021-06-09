using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PPProject.Client.Services
{
    public interface ICartService
    {
        public Task<Cart> GetCartAsync();
        public Task<HttpResponseMessage> RemoveItemAsync(int itemId);
        public Task<HttpResponseMessage> AddItemAsync(int productId,int quantity);
        public Task<HttpResponseMessage> DiscardCartAsync(Cart cart);
        public Task<HttpResponseMessage> LogDiscardedCartAsync(Cart cart);
    }
}
