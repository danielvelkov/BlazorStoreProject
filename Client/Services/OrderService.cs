using Newtonsoft.Json;
using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PPProject.Client.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient httpClient;
        public OrderService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> DeleteOrder(int orderId) =>
            await httpClient.DeleteAsync($"api/orders/{orderId}");


        public async Task<Order> GetOrder(int orderId) =>
            await httpClient.GetFromJsonAsync<Order>($"api/orders/{orderId}");


        public async Task<IEnumerable<Order>> GetUserOrders() =>
            await httpClient.GetFromJsonAsync<Order[]>($"api/orders/user");

        public async Task<IEnumerable<Order>> GetOrders() =>
            await httpClient.GetFromJsonAsync<Order[]>($"api/orders");

        public async Task<int> PlaceOrder(Order order)
        {
            var serialized = JsonConvert.SerializeObject(order);
            var stringContent = new StringContent(serialized, Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync($"api/orders", stringContent);
            result.EnsureSuccessStatusCode();
            //var error = result.Content.ReadAsStringAsync().Result;
            //Console.WriteLine(error);
            return await result.Content.ReadFromJsonAsync<int>();

        }

        public async Task<Order> GetOrderAsAdmin(int orderId)
        {
            return await httpClient.GetFromJsonAsync<Order>($"api/orders/admin_view/{orderId}");
        }
    }
}
