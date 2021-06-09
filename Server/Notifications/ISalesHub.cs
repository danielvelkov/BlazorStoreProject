using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Notifications
{
    public interface ISalesHub
    {
        public Task OrderCreated(Order order);
        public Task ItemAddedToCart(Product product);
        public Task ItemRemovedFromCart(Product product);
        public Task CartDiscarded(Cart cart);
    }
}
