using Bogus;
using Bogus.Extensions;
using PPProject.Server.Models;
using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Data
{
    public static class FakeData
    {

        public static List<Product> Products = new();
        public static List<Cart> Carts = new();
        public static List<Cart> DiscardedCarts = new();
        public static List<CartItem> CartItems = new();
        public static List<Order> Orders = new();
        public static List<Address> Addresses = new();

        public static void InitFakeOrders(int numFakeProducts, 
            int numFakeOrders,
            int numOfDiscardedFakeCarts )
        {
            var productFaker = new Faker<Product>()
               .RuleFor(p=> p.Name, f=>f.Commerce.Product())
               .RuleFor(p => p.Price, f => f.Random.Float(5,200))
               .RuleFor(p => p.Description,f=> f.Commerce.ProductDescription())
               .RuleFor(p => p.Quantity, f => f.Random.Number(0,10))
               .RuleFor(p => p.CreatedAt, f => f.Date.Past())
               .RuleFor(p => p.Discount, f => f.Random.Float(0,0.99f))
               .RuleFor(p => p.Discount_Exp,f=> f.Date.Past())
               .RuleFor(p => p.CategoryId,f=> f.Random.Number(1,3)) //this is hardcoded hmmm kinda bad
               .RuleFor(p => p.Name, f => f.Commerce.Product());

            var products= productFaker.Generate(numFakeProducts);
            Products.AddRange(products);

            var cartItemFaker = new Faker<CartItem>()
              .RuleFor(ci => ci.Discount, f => f.Random.Float(0, 0.99f))
              .RuleFor(ci => ci.Quantity, f => f.Random.Number(1, 10))
              .RuleFor(ci => ci.ProductId, f => f.PickRandom(Enumerable.Range(1,numFakeProducts)));


            var checkedOutCartsFaker = new Faker<Cart>()
               .RuleFor(c => c.UserId, f => f.Random.AlphaNumeric(9)) //past me gj not making this a hard foreign relationship
               .RuleFor(c => c.Status, f => Status.CHECKED_OUT)
               .RuleFor(c => c.CreatedAt, f => f.Date.Past())
               .RuleFor(c => c.Items, (f, c) =>
               {
                   cartItemFaker.RuleFor(ci => ci.CartId, _ => c.ID);

                   var cartItems = cartItemFaker.GenerateBetween(1, 6);

                   CartItems.AddRange(cartItems);

                   return cartItems;
               });

            var carts = checkedOutCartsFaker.Generate(numFakeOrders); // orders and checked out carts a 1:1 relationship
            Carts.AddRange(carts);

            var addressFaker = new Faker<Address>()
              .RuleFor(a => a.Region, f => f.Address.County())
              .RuleFor(a => a.City, f => f.Address.City())
              .RuleFor(a => a.PostalCode, f => f.Address.CountryCode())
              .RuleFor(a => a.Line1, f => f.Address.StreetAddress())
              .RuleFor(a => a.Line2, f => f.Address.StreetAddress());

            var addresses = addressFaker.Generate(5);
            Addresses.AddRange(addresses);

            int cartId = 0;

            var orderFaker = new Faker<Order>()
              .RuleFor(o => o.Billing, f => f.Random.Float(1, 500))
              .RuleFor(o => o.Issued, f => f.Date.Past())
              .RuleFor(o => o.AddressId, f => f.PickRandom(Enumerable.Range(1, 5))) //we added 5 addresses
              .RuleFor(o => o.Cart, f => Carts.ElementAt(cartId++));

            var orders = orderFaker.Generate(numFakeOrders);
            Orders.AddRange(orders);

            var discardedCartsFaker = new Faker<Cart>()
               .RuleFor(c => c.UserId, f => f.Random.AlphaNumeric(9)) //past me gj not making this a hard foreign relationship
               .RuleFor(c => c.Status, f => Status.DISCARDED)
               .RuleFor(c => c.CreatedAt, f => f.Date.Past())
               .RuleFor(c => c.Items, (f, c) =>
               {
                   cartItemFaker.RuleFor(ci => ci.CartId, _ => c.ID);

                   var cartItems = cartItemFaker.GenerateBetween(1, 6);

                   CartItems.AddRange(cartItems);

                   return cartItems;
               });

            var discardedCarts = discardedCartsFaker.Generate(numOfDiscardedFakeCarts);
            DiscardedCarts.AddRange(discardedCarts);
        }
    }
}
