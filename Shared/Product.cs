using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace PPProject.Shared
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, Range(0.01, 9999)]
        public float Price { get; set; }
        [Required, Range(0, 9999)]
        public int Quantity { get; set; }
        [MaxLength(10000), Display(Name = "Product Description"), DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }
        [Range(0, .99)]
        public float Discount { get; set; }

        public DateTime Discount_Exp { get; set; }

        public FileImage ProductPic { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public Product(
                      string name,
                      float price,
                      int quantity,
                      string description,
                      float discount,
                      FileImage productPic,
                      int categoryId)
        {
            this.Name = name;
            this.Price = price;
            this.Quantity = quantity;
            this.Description = description;
            this.Discount = discount;
            this.ProductPic = productPic;
            this.CategoryId = categoryId;
        }

        public Product() { }

        public Product(Product product)
        {
            ID = product.ID;
            CategoryId = product.CategoryId;
            Price = product.Price;
            ProductPic = product.ProductPic;
            Discount = product.Discount;
            Discount_Exp = product.Discount_Exp;
            Description = product.Description;
            CreatedAt = product.CreatedAt;
            Name = product.Name;
            Quantity = product.Quantity;
        }

        public string GetFormattedPrice() => Price.ToString("0.00");

        public string GetImageData()
        {
            if (ProductPic != null)
                return $"data:{Path.GetExtension(ProductPic.FileName)};" +
            $"base64,{Convert.ToBase64String(ProductPic.Content ?? Array.Empty<byte>())}";
            else return null;

        }
        public string GetFormattedPriceWithDiscount() => DiscountedPrice().ToString("0.00");

        public float DiscountedPrice() => (Price - Price * (Discount));

        public bool DiscountExpired() => Discount_Exp.CompareTo(DateTime.Now) <= 0;

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Product p = (Product)obj;
                return ID == p.ID && (Price == p.Price) && (Discount == p.Discount) && (Discount_Exp == p.Discount_Exp);
            }
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}