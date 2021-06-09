using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPProject.Server.Controllers.HelperClasses;
using PPProject.Server.Data;
using PPProject.Server.Models;
using PPProject.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public SalesController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            _context = context;
            userManager = manager;
        }

        // GET: api/Orders
        [HttpGet("categories")]
        public async Task<ActionResult<IDictionary<string, float>>> GetSalePercentageForCategory()
        {
            var itemSoldPerCategory = new Dictionary<string, float>();
            await _context.CartItems.
                 Where(ci => ci.Cart.Status == Status.CHECKED_OUT).
                 Include(ci => ci.Product).ThenInclude(p => p.Category).GroupBy(ci => ci.Product.Category.Name)
                         .Select(group => new
                         {
                             Category = group.Key,
                             TimesBought = group.Count()
                         })
                         .ForEachAsync((cp) => itemSoldPerCategory.Add(cp.Category, cp.TimesBought));
            float totalSold = 0;
            Parallel.ForEach(itemSoldPerCategory, (i) =>
            {
                totalSold += i.Value;
            });
            // make them a percentage
            Parallel.ForEach(itemSoldPerCategory, (pair) =>
              {
                  itemSoldPerCategory[pair.Key] /= totalSold;
              });

            // IDK about this but it seems legit. Its there cuz there might be no sales
            foreach (var category in _context.Categories.ToList())
                itemSoldPerCategory.TryAdd(category.Name, 0);


            return itemSoldPerCategory;
        }

        // GET: api/sales/by_month
        [HttpGet("by_month")]
        public async Task<ActionResult<IDictionary<DateTime, int>>> GetSalesByMonth()
        {
            var now = DateTime.Now;
            now = now.Date.AddDays(1 - now.Day);
            var months = Enumerable.Range(-12, 13)
                .Select(x => new
                {
                    year = now.AddMonths(x).Year,
                    month = now.AddMonths(x).Month
                });
            var salesPerMonth = new Dictionary<DateTime, int>();

            try
            {
                await Task.Run(() =>
                {
                    salesPerMonth =
                        months.GroupJoin(_context.Orders.Include(o => o.Cart).ThenInclude(c => c.Items).ToList(),
                            m => new { month = m.month, year = m.year },
                            order => new
                            {
                                month = order.Issued.Month,
                                year = order.Issued.Year
                            },
                            (p, g) => new
                            {
                                date = new DateTime(p.year, p.month, 1),
                                count = g.Sum(a => a.Cart.Items.Count)
                            }).ToDictionary(o => o.date, o => o.count);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Problem();
            }

            return salesPerMonth;
        }

        [HttpGet("most_sold/today")]
        public async Task<ActionResult<IDictionary<int, Tuple<string, int>>>> GetProductsThatSoldWellToday()
        {
            var today = DateTime.Today;

            var mostSelledProductsForToday = new Dictionary<int, Tuple<string, int>>();

            try
            {
                await Task.Run(() =>
                {
                    var ordersMadeToday = _context.Orders
                         .Where(o => o.Issued.DayOfYear == today.DayOfYear)
                        .Include(o => o.Cart).ThenInclude(c => c.Items).ThenInclude(ci => ci.Product).AsEnumerable(); // if you dont aadd this enumerable whole thing breaks for some reason

                    mostSelledProductsForToday = ordersMadeToday
                       .SelectMany(o => o.Cart.Items).
                       GroupBy(x => x.ProductId).
                       Select(x => new
                       {
                           Id = x.Key,
                           Sales = new Tuple<string, int>(
                           _context.Products.FirstOrDefault(p => p.ID == x.Key).Name, x.Count())
                       })
                        .OrderByDescending(x => x.Sales.Item2).ToDictionary(p => p.Id, p => p.Sales);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Problem();
            }

            return mostSelledProductsForToday;
        }

        [HttpGet("most_sold/this_month")]
        public async Task<ActionResult<IDictionary<int, Tuple<string, int>>>> GetProductsThatSoldWellThisMonth()
        {
            var today = DateTime.Today;

            var mostSelledProductsForToday = new Dictionary<int, Tuple<string, int>>();

            try
            {
                await Task.Run(() =>
                {
                    var ordersMadeToday = _context.Orders
                         .Where(o => o.Issued.Month == today.Month)
                        .Include(o => o.Cart).ThenInclude(c => c.Items).ThenInclude(ci => ci.Product).AsEnumerable(); // if you dont aadd this enumerable whole thing breaks for some reason

                    mostSelledProductsForToday = ordersMadeToday
                       .SelectMany(o => o.Cart.Items).
                       GroupBy(x => x.ProductId).
                       Select(x => new
                       {
                           Id = x.Key,
                           Sales = new Tuple<string, int>(
                           _context.Products.FirstOrDefault(p => p.ID == x.Key).Name, x.Count())
                       })
                        .OrderByDescending(x => x.Sales.Item2).ToDictionary(p => p.Id, p => p.Sales);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Problem();
            }

            return mostSelledProductsForToday;
        }
    }


}

