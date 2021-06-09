using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPProject.Shared
{
    public class CartItem
    {
        [Key]
        public int ID { get; set; }
        public int ProductId { get; set; }
        public int CartId { get; set; }
        [Range(1, 9999)]
        public int Quantity { get; set; }
        [Range(0.00, 0.99)]
        public float Discount { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; }

        public float GetTotal() => Product.Price * Quantity;

        public float GetTotalWithDiscount() => (Product.Price - (Product.Price * Discount)) * Quantity;

        public string GetTotalFormatted() => GetTotal().ToString("0.00");

        public string GetTotalWithDiscountFormatted() => GetTotalWithDiscount().ToString("0.00");
    }
}