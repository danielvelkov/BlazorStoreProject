using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PPProject.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Controllers.HelperClasses
{
    public static class ControllerBaseExtension
    {
        public static async Task<string> GetUserIdAsync(this ControllerBase controllerBase,UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.GetUserAsync(controllerBase.User);
            return user.Id;
        }
    }
}
