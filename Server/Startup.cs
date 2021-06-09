using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PPProject.Server.Data;
using PPProject.Server.Models;
using PPProject.Server.Notifications;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace PPProject.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            // added by ASP.NET Core Identity
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>() // this code adds a role based security. see more at: https://code-maze.com/using-roles-in-blazor-webassembly-hosted-applications/
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // if this looks like magic to you see: https://code-maze.com/using-roles-in-blazor-webassembly-hosted-applications/
            services.AddIdentityServer()
                  .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(opt =>
                  {
                      opt.IdentityResources["openid"].UserClaims.Add("name");
                      opt.ApiResources.Single().UserClaims.Add("name");
                      opt.IdentityResources["openid"].UserClaims.Add("role");
                      opt.ApiResources.Single().UserClaims.Add("role");
                  });
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

            // code added for using the ClaimsType User in the controllerbase classes
            services.Configure<IdentityOptions>(options =>
                 options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

            services.AddAuthentication()
                .AddIdentityServerJwt();
            // see more at: https://docs.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/hosted-with-identity-server?view=aspnetcore-5.0&tabs=visual-studio
            services.AddControllersWithViews();
            services.AddRazorPages()
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                }); //we add this because GET requests with nested items arent handled well with http.json

            // this will handle notifications https://www.dotnetcurry.com/aspnet-core/realtime-app-using-blazor-webassembly-signalr-csharp9
            services.AddSignalR();

            // this is for seeing the controller methods
            // uncomment and go to swagger/index.html
            //services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            ApplicationDbContext ctx, IServiceProvider serviceProvider)
        {
            new SeedData(ctx).InitializeAsync(serviceProvider).Wait();

            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store Api V1");
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseAuthentication();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<SalesHub>("/saleshub");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
