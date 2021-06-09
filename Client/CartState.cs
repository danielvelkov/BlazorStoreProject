using PPProject.Client.Services;
using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PPProject.Client
{
    public class CartState
    {
        private const int MAX_CART_CAPACITY = 10;

        public Cart Cart { get; private set; } = new Cart
        {
            Items = new List<CartItem>(MAX_CART_CAPACITY)
        };

        public void AddProductToTempCart(Product product, int buyingQuantity)
        {
            if (Cart.Items.Count == MAX_CART_CAPACITY - 1)
                throw new Exception($"Cart can have a max. of {MAX_CART_CAPACITY-1} items");
            if (Cart.Items.AsQueryable().FirstOrDefault(ci => ci.Product == product) != null)
            {
                Cart.Items.First(ci => ci.Product == product).Quantity += buyingQuantity;
                return;
            }
            Cart.Items.Add(new CartItem
            {
                Product = product,
                Discount = product.Discount,
                Quantity = buyingQuantity

            });
        }

        public void AddItemsToRealCart(Cart userCart,ICartService cartService)
        {
            foreach (CartItem item in Cart?.Items)
            {
                if (userCart.Items == null)
                    userCart.Items = new List<CartItem>(MAX_CART_CAPACITY);
                // items may not be initialized so we return false if empty
                if (userCart.Items?.Contains(item, new CartItemEqualityComparer()) ?? false)
                    userCart.Items.First(x => x.ProductId == item.ProductId).Quantity += item.Quantity;
                else
                {
                    if (userCart.Items.Count == MAX_CART_CAPACITY - 1)
                        throw new Exception("CART was already full. Cant add anymore");
                    userCart.Items.Add(item);
                    cartService.AddItemAsync(item.ProductId, item.Quantity);
                }
            }
        }

        public void RemoveItem(CartItem cartItem)
        {
            Cart.Items.Remove(cartItem);
        }

        public void EmptyCart()
        {
            Cart = new Cart
            {
                Items = new List<CartItem>(MAX_CART_CAPACITY)
            };
        }

        public void ReplaceCart(Cart cart)
        {
            Cart = cart;
        }
    }
}
