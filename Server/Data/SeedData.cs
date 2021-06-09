using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PPProject.Server.Models;
using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Data
{
    public class SeedData
    {
        private readonly ApplicationDbContext context;

        public SeedData(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {

            string[] roles = new string[]
            {
                "Administrator",
                "Customer"
            };

            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Look for any orders.
            if (context.Orders.Any())
            {
                return;   // DB has already been seeded
            }


            ApplicationUser admin = context.Users.SingleOrDefault(u => u.Email == "admin@gmail.com");
            if (admin == null)
            {
                admin = new ApplicationUser()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Big",
                    LastName = "Boss",
                    PhoneNumber = "+088983455631",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    Address = new Address
                    {
                        City = "Sliven",
                        Region = "Sliven",
                        Line1 = "whatever street, whatever num",
                        PostalCode = "8800"
                    }
                };
                await UserManager.CreateAsync(admin, "Admin_1234");
            }
            // Add Roles
            await UserManager.AddToRoleAsync(admin, roles[0]);
            await UserManager.AddToRoleAsync(admin, roles[1]);

            // Create Customer
            ApplicationUser user = await UserManager.FindByEmailAsync("user@gmail.com");

            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = "user@gmail.com",
                    Email = "user@gmail.com",
                    FirstName = "Billy",
                    LastName = "Bob",
                    PhoneNumber = "+088172455631",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    Address = new Address
                    {
                        City = "Sofia",
                        Region = "Sofia",
                        Line1 = "amogus strt.",
                        PostalCode = "1000"
                    }

                };
                await UserManager.CreateAsync(user, "User_1234");
            }
            await UserManager.AddToRoleAsync(user, roles[1]);

            context.SaveChanges();

            // Look for any categories.
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
               new Category
               {
                   Name = "Television",
                   PromoCode = "TV",
                   PromoCode_Exp = System.DateTime.Now
               },

               new Category
               {
                   Name = "SmartPhones",
                   PromoCode = "PHONE",
                   PromoCode_Exp = System.DateTime.Now
               },

               new Category
               {
                   Name = "Computers",
                   PromoCode = "COMP",
                   PromoCode_Exp = System.DateTime.Now
               }
          );

                context.SaveChanges();
            }



            //context.Products.AddRange(
            //    new Product
            //    {
            //        Name = "Panasonic LED TV",
            //        Price = 400.00F,
            //        Quantity = 10,
            //        Description = "its a tv",
            //        CreatedAt = DateTime.Now,
            //        Discount = 0,
            //        Discount_Exp = new DateTime(),
            //        ProductPic = null,
            //        CategoryId = 1
            //    },
            //    new Product
            //    {
            //        Name = "Apple IPhone 8",
            //        Price = 400.00F,
            //        Quantity = 100,
            //        Description = "Steve would be proud",
            //        CreatedAt = DateTime.Now,
            //        Discount = 0,
            //        Discount_Exp = new DateTime(),
            //        ProductPic = null,
            //        CategoryId = 2
            //    },
            //    new Product
            //    {
            //        Name = "Omen HP Laptop",
            //        Price = 400.00F,
            //        Quantity = 15,
            //        Description = "it rocks",
            //        CreatedAt = DateTime.Now,
            //        Discount = 0,
            //        Discount_Exp = new DateTime(),
            //        ProductPic = null,
            //        CategoryId = 3
            //    }
            //);
            //context.SaveChanges();


            FakeData.InitFakeOrders(50, 300, 100);

            int addressCount = context.Addresses.Count();
            if (addressCount == 2)
                await context.Addresses.AddRangeAsync(FakeData.Addresses);

            int productsCount = context.Products.Count();
            if (productsCount == 0)
                await context.Products.AddRangeAsync(FakeData.Products);

            context.SaveChanges();

            int cartsCount = context.Carts.Count();
            if (cartsCount == 0)
            {
                await context.Carts.AddRangeAsync(FakeData.Carts);

                context.SaveChanges();
            }

            int orderCount = context.Orders.Count();
            if (orderCount == 0)
            {
                await context.Orders.AddRangeAsync(FakeData.Orders);

                context.SaveChanges();
            }

            int discardedCartsCount = context.Carts.Where(c => c.Status == Status.DISCARDED).Count();
            if (discardedCartsCount == 0)
                await context.Carts.AddRangeAsync(FakeData.DiscardedCarts);

            context.SaveChanges();

        }


    }

}