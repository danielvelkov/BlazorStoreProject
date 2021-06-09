using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPProject.Shared
{
    public class Address
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Line1 { get; set; }
        
        [MaxLength(100)]
        public string Line2 { get; set; }

        [Required, MaxLength(50)]
        public string City { get; set; }

        [Required, MaxLength(20)]
        public string Region { get; set; }

        [Required, MaxLength(20)]
        public string PostalCode { get; set; }

        public Address() { }

        public override string ToString()
        {
            StringBuilder address = new();
            address.Append($"{Line1}\n");
            address.Append($"{City} ");
            address.Append($"{PostalCode}\n");
            address.Append($"({Region}) BULGARIA");
            return address.ToString();

        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Address a = (Address)obj;
                return Line1 == a.Line1 && (City == a.City) && (PostalCode == a.PostalCode) && (Region == a.Region) && (Line2 == a.Line2);
            }
        }
    }
}
