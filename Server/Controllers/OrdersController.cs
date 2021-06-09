using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PPProject.Server.Controllers.HelperClasses;
using PPProject.Server.Data;
using PPProject.Server.Models;
using PPProject.Server.Notifications;
using PPProject.Shared;

namespace PPProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IHubContext<SalesHub, ISalesHub> hubContext;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public OrdersController(ApplicationDbContext context, 
            UserManager<ApplicationUser> manager,
             IHubContext<SalesHub, ISalesHub> salesHub)
        {
            _context = context;
            userManager = manager;
            hubContext = salesHub;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.
                Include(o => o.Address).
                Include(o => o.Cart).
                ThenInclude(c =>c.Items).
                ThenInclude(i=>i.Product).
                ToListAsync();
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders()
        {
            var userId = await this.GetUserIdAsync(userManager);
            return await _context.Orders.Where(o => o.Cart.UserId == userId).
                Include(o => o.Address).
                Include(o => o.Cart).
                ThenInclude(c => c.Items).
                ThenInclude(i => i.Product).
                ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            //anyone can get an order. thats kinda bad
            //so we add the user id
            var userId = await this.GetUserIdAsync(userManager);
            var order = await _context.Orders.Where(o=>o.Cart.UserId==userId).
                Include(o => o.Cart).
                    ThenInclude(c => c.Items).
                        ThenInclude(i => i.Product).
                FirstOrDefaultAsync(o=>o.ID==id);

            order.Address = await _context.Addresses.FirstAsync(a => a.Id == order.AddressId);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // GET: api/Orders/5
        [HttpGet("admin_view/{id}")]
        public async Task<ActionResult<Order>> GetOrderAsAdmin(int id)
        {
            var order = await _context.Orders.
                Include(o => o.Cart).
                    ThenInclude(c => c.Items).
                        ThenInclude(i => i.Product).
                FirstOrDefaultAsync(o => o.ID == id);

            order.Address = await _context.Addresses.FirstAsync(a => a.Id == order.AddressId);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.ID)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // now that i think about it, post requests should be queued somehow so they do not interfere. with rabbitmq or something.
        // and in the orders tab of the user it will show processing so he can continue browsing the site

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<int>> PostOrder(Order order)
        {
            if (order == null)
            {
                return BadRequest("Order can't be null");
            }
            order.Issued = DateTime.Now;
            // check if the products actually exist
            var productsAndCartItems = _context.Products.ToList().
                Join(order.Cart.Items.ToList(),
                p => p.ID,
                i => i.Product.ID,
                (p, i) => new { Product = p, Item = i }).ToList();

            if (productsAndCartItems.Count != order.Cart.Items.Count)
            {
                return Conflict("Some of these products are no longer present in the shop.");
            }

            // check if the prices have not been tampered
            //var productsAndCartItems = products.Zip(order.Cart.Items, (p, ci) => new { Product = p, CartItem = ci });
            foreach (var pair in productsAndCartItems)
            {
                bool productsAreEqual = pair.Item.Product.Equals(pair.Product);
                bool discountsAreEqual = pair.Item.Discount == pair.Product.Discount;
                if (!productsAreEqual || !discountsAreEqual)
                {
                    return Conflict("Prices have changed.");
                }
            }

            // generate billing
            float billing = 0;
            foreach (var item in order.Cart.Items)
            {
                if (item.Discount > 0)
                    billing += item.GetTotalWithDiscount();
                else
                    billing += item.GetTotal();
            }

            order.Billing = billing;

            // check if address of order exists
            var user = await userManager.GetUserAsync(HttpContext.User);
            var address = _context.Addresses.Where(a => a.Id == user.AddressId).FirstOrDefault();
            if (address != null)
            {
                // if the entered address is different then change the default one too
                if (!address.Equals(order.Address))
                {
                    address = order.Address;

                    _context.Entry(order.Address).State = EntityState.Detached;
                    _context.Entry(user.Address).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                }

                _context.Entry(address).State = EntityState.Detached;
            }
            else
                return BadRequest("User has no address");

            order.Cart.Status = Status.CHECKED_OUT;

            _context.Entry(order.Cart).State = EntityState.Modified;


            foreach (var tuple in productsAndCartItems)
            {
                tuple.Product.Quantity -= tuple.Item.Quantity;
            }
            await _context.SaveChangesAsync();

            foreach (var tuple in productsAndCartItems)
            _context.Entry(tuple.Product).State = EntityState.Detached;

            _context.Orders.Attach(order);
            await _context.SaveChangesAsync();

            //TODO: omg im retarded. when you sell the product quantity gooes down

            await hubContext.Clients.All.OrderCreated(order);
            return order.ID;
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.ID == id);
        }
    }
}
