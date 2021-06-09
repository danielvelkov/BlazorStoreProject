using Microsoft.AspNetCore.Identity;
using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Address Address { get; set; }

        public int? AddressId { get; set; }

        [DataType(DataType.Date)] //we only take date; not hours
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
    }
}
