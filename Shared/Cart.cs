using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPProject.Shared
{
    public class Cart
    {
        [Key]
        public int ID { get; set; }

        public string UserId { get; set; }

        public Status Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<CartItem> Items { get; set; }

        public float GetTotalPrice()
        {
            float sum = 0;
            if(Items!=null)
            foreach (var item in Items)
                sum += item.Product.Price * item.Quantity;
            return sum;
        }

        public float GetDiscountedTotalPrice()
        {
            {
                float sum = 0;
                if (Items != null)
                    foreach (var item in Items)
                    {
                        if (!item.Product.DiscountExpired())
                            sum += item.Product.DiscountedPrice() * item.Quantity;
                        else sum += item.Product.Price * item.Quantity;
                    }
                return sum;
            }
        }

        [NotMapped]
        public string GetFormattedTotalPrice => GetTotalPrice().ToString("0.00");

        [NotMapped]
        public string GetFormattedDiscountedPrice => GetDiscountedTotalPrice().ToString("0.00");

        

        public override string ToString()
        {
            StringBuilder products = new();
            foreach (var item in Items)
            {
                products.Append(item.Product.Name);
                if (item.Quantity > 1)
                    products.Append($" x {item.Quantity}");
                products.Append(" | ");
            }
            return products.ToString();
        }
    }

    public enum Status
    {
        ACTIVE,
        CHECKED_OUT,
        DISCARDED
    }

    public class CartItemEqualityComparer : IEqualityComparer<CartItem>
    {
        public bool Equals(CartItem x, CartItem y)
        {
            if (x.ProductId == y.ProductId)
                return true;
            else return false;
        }

        public int GetHashCode([DisallowNull] CartItem obj)
        {
            throw new NotImplementedException();
        }
    }
}
