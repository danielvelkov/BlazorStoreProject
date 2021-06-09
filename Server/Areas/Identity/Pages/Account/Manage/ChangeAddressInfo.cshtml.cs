using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPProject.Server.Data;
using PPProject.Server.Models;
using PPProject.Shared;

namespace PPProject.Server.Areas.Identity.Pages.Account.Manage
{
    public partial class ChangeAddressInfoModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly ApplicationDbContext context;

        public ChangeAddressInfoModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            context = dbContext;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            // address details

            [Required]
            [StringLength(100)]
            [Display(Name = "Line 1")]
            public string Line1 { get; set; }

            [StringLength(100)]
            [Display(Name = "Line 2 (optional)")]
            public string Line2 { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [StringLength(20)]
            [Display(Name = "Region")]
            public string Region { get; set; }

            [Required]
            [MaxLength(20)]
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var address = await context.Addresses.FindAsync(user.AddressId);
            user.Address = address;

            Input = new InputModel
            {
                // address details
                Line1 = user.Address?.Line1,
                Line2 = user.Address?.Line2,
                City = user.Address?.City,
                Region = user.Address?.Region,
                PostalCode = user.Address?.PostalCode
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // address info update (my edit)
            user.Address = new Address
            {
                Region = Input.Region,
                City = Input.City,
                Line1 = Input.Line1,
                Line2 = Input.Line2,
                PostalCode = Input.PostalCode,
            };

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                StatusMessage = "Unexpected error when trying change address info.";
                return RedirectToPage();
            }
            //
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }

}
