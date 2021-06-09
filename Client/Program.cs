using Blazored.Toast;
using BlazorStrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PPProject.Client.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PPProject.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // default HttpClient handles authorization-required pages and functions
            builder.Services.AddHttpClient("PPProject.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()
                ;


            // this visitor client handles stuff for anonymous users like product browsing and other unathenticated func
            builder.Services.AddHttpClient<PublicClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
                //.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>(); //by commenting this we omit the req for authorization tokens

            //// Supply HttpClient instances that include access tokens when making requests to the server project
            ///TLDR: this makes it so the baseaddress of the requests are of that of the server api project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("PPProject.ServerAPI"));


            // adding auth services

            builder.Services.AddApiAuthorization<CustomerAuthenticationState>(options =>
            {
                options.AuthenticationPaths.LogOutSucceededPath = ""; //logout redirects to home page
            })
                .AddAccountClaimsPrincipalFactory<CustomerAuthenticationState, CustomUserFactory>();
            // this allows the role claims to be split in the jwt so the authorize view notices them
            // we need them split cuz the owner of the store is set to be an ADMIN AND CUSTOMER

            builder.Services.AddSingleton<CartState>();
            //even if we do addscoped it still acts like singleton
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IUserInfoService, UserInfoService>();
            builder.Services.AddScoped<ISalesService, SalesService>();

            builder.Services.AddBootstrapCss();

            // for toast/snackbar notification messages
            builder.Services.AddBlazoredToast();

            // this defines the signalr connection but doesnt start it
            builder.Services.AddSingleton<HubConnection>(sp => {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                return new HubConnectionBuilder()
                  .WithUrl(navigationManager.ToAbsoluteUri("/saleshub"))
                  .WithAutomaticReconnect()
                  .Build();
            });

            await builder.Build().RunAsync();
        }
    }
}
