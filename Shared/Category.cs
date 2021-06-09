using System;
using System.ComponentModel.DataAnnotations;

namespace PPProject.Shared
{
    public class Category
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string PromoCode { get; set; }
        public DateTime PromoCode_Exp { get; set; }

        public Category(int iD, string name, string promoCode, DateTime promoCode_Exp)
        {
            ID = iD;
            Name = name;
            PromoCode = promoCode;
            PromoCode_Exp = promoCode_Exp;
        }

        public Category() { }

    }
}