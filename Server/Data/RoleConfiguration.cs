using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Data
{
    public class RoleConfiguration: IEntityTypeConfiguration<IdentityRole>
    {
        //https://code-maze.com/using-roles-in-blazor-webassembly-hosted-applications/
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
               
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                },
                new IdentityRole
                {
                    Name = "Customer",
                    NormalizedName = "CUSTOMER"
                }
            );
        }
    }
}
