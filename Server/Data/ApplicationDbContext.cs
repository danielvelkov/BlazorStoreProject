using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PPProject.Server.Models;
using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<FileImage> FileImage { get; set; }

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        // this adds role based security
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
             .HasOne(u => u.Address)
             .WithOne()
             .HasForeignKey<ApplicationUser>(c => c.AddressId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Cascade);


            //for bogus to work we need to start at 1
            builder.Entity<Product>()
                .Property(x => x.ID)
                .UseIdentityColumn(seed: 1, increment: 1);

            builder.Entity<FileImage>()
                .Property(x => x.Id)
                .UseIdentityColumn(seed: 1, increment: 1);

            builder.Entity<Address>()
                .Property(x => x.Id)
                .UseIdentityColumn(seed: 1, increment: 1);

            builder.Entity<CartItem>()
                .Property(x => x.ID)
                .UseIdentityColumn(seed: 1, increment: 1);

            builder.Entity<Cart>()
                .Property(x => x.ID)
                .UseIdentityColumn(seed: 1, increment: 1);

            builder.Entity<Order>()
                .Property(x => x.ID)
                .UseIdentityColumn(seed: 1, increment: 1);

            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
