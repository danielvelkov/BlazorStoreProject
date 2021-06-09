using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Client
{
    public class CustomerAuthenticationState: RemoteAuthenticationState
    {
        public Cart Cart { get; set; }
    }
}
