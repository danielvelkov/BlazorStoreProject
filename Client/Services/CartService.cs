using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PPProject.Client.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient httpClient;
        private readonly PublicClient publicHttpClient;

        public CartService(HttpClient httpClient,PublicClient publicClient)
        {
            this.httpClient = httpClient;
            publicHttpClient = publicClient;
        }

        public async Task<Cart> GetCartAsync() =>
            await httpClient.GetFromJsonAsync<Cart>($"api/mycart");

        public async Task<HttpResponseMessage> RemoveItemAsync(int itemId) =>
            await httpClient.DeleteAsync($"api/mycart/Remove/{itemId}");

        public async Task<HttpResponseMessage> AddItemAsync(int productId, int quantity) =>
             await httpClient.GetAsync($"api/mycart/Add/{productId}/quantity/{quantity}");
        
        public async Task<HttpResponseMessage> DiscardCartAsync(Cart cart) =>
             await httpClient.PutAsJsonAsync($"api/mycart/{cart.ID}",cart);

        public async Task<HttpResponseMessage> LogDiscardedCartAsync(Cart cart)
        {
            cart.Status = Status.DISCARDED;
            return await publicHttpClient.Client.PostAsJsonAsync("api/mycart", cart);
        }
    }
}
