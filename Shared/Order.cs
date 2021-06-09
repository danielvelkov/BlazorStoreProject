using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPProject.Shared
{
    public class Order
    {
        [Key]
        public int ID { get; set; }

        public int CartId { get; set; }
        
        public int AddressId { get; set; }

        [Range(0.01, 99999)]
        public float Billing { get; set; }

        public DateTime Issued { get; set; }

        [Required]
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; }

        [Required]
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }

        public Order(int cartId, 
            int addressId, 
            float billing, 
            DateTime createdAt)
        {
            CartId = cartId;
            AddressId = addressId;
            Billing = billing;
            Issued = createdAt;
        }

        public Order() 
        {
        }

    }
}
