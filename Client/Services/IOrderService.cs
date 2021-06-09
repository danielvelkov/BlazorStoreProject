using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PPProject.Client.Services
{
    public interface IOrderService
    {
        public Task<int> PlaceOrder(Order order);
        public Task<Order> GetOrder(int orderId);
        public Task<Order> GetOrderAsAdmin(int orderId);
        public Task<HttpResponseMessage> DeleteOrder(int orderId);
        public Task<IEnumerable<Order>> GetUserOrders();
        public Task<IEnumerable<Order>> GetOrders();
    }
}
