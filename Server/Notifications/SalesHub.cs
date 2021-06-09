using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Notifications
{
    public class SalesHub: Hub<ISalesHub>
    {
        // dont implement the methods. That is for the client
        // that will receive the messages to do
    }
}
