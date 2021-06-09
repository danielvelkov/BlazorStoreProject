using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
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
    public class MyCartController : ControllerBase
    {
        private readonly IHubContext<SalesHub, ISalesHub> hubContext;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public MyCartController(ApplicationDbContext context,
            UserManager<ApplicationUser> usrManager,
            IHubContext<SalesHub, ISalesHub> salesHub)
        {
            _context = context;
            userManager = usrManager;
            hubContext = salesHub;
        }

        // GET: api/MyCart
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Cart>> GetCart()
        {
            // this is kinda cool. we can request a sessionId so we can distinguish if session changed
            //var sessionCookie = HttpContext.Request.Cookies[IdentityServerConstants.ExternalCookieAuthenticationScheme];
            //if (sessionCookie != null) ;

            var userId = await this.GetUserIdAsync(userManager);
            var cart= await _context.Carts.Where(c=>c.UserId==userId && c.Status==Status.ACTIVE).
                Include(c => c.Items).
                ThenInclude(i => i.Product).
                FirstOrDefaultAsync();


            //if there is no cart OR no active cart => make a new one
            if(cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Status = Status.ACTIVE
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }

       
        // PUT: api/MyCart/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut("{ID}")]
        [Authorize]
        public async Task<IActionResult> DiscardCart(int id, Cart cart)
        {
            if (id != cart.ID)
            {
                return BadRequest("Cart ID mismatch");
            }
            // discard the cart
            cart.Status = Status.DISCARDED;
            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound($"Cart with Id = {id} not found");
                }
                else
                {
                    throw;
                }
            }

            await hubContext.Clients.All.CartDiscarded(cart);
            return NoContent();
        }

        [HttpGet("Add/{productId}/quantity/{quantity}")]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId,int quantity)
        {
            // Retrieve the product from the database
            var addedProduct = _context.Products
                .Single(product => product.ID == productId);

            // Add it to the shopping cart
            var cart = await GetUserCartAsync();

            var cartItem = cart.Items.FirstOrDefault(c => c.ProductId == productId && c.CartId==cart.ID);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Discount = addedProduct.Discount,
                    Quantity = quantity,
                    CartId = cart.ID
                };
                await _context.AddAsync(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }
            await hubContext.Clients.All.ItemAddedToCart(addedProduct); //this will notify the client of the activity
            await _context.SaveChangesAsync(HttpContext.RequestAborted);
            return NoContent();
        }

        // we add the discarded cart for further analysis >:) 
        // POST: api/MyCart
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
            try
            {
                cart.UserId = "ANONYMOUS";
                foreach(var ci in cart.Items)
                    _context.Entry(ci.Product).State = EntityState.Detached;

                await _context.SaveChangesAsync();

                _context.Carts.Attach(cart);
                await _context.SaveChangesAsync();
                await hubContext.Clients.All.CartDiscarded(cart);
            }
            catch(Exception e)
            {
                return Problem(e.InnerException.Message);
            }
            return Ok();
        }

        // DELETE: api/MyCart/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/MyCart/Remove/5
        [HttpDelete("Remove/{cartItemId}")]
        [Authorize]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var cart = await GetUserCartAsync();
            if (cart == null)
            {
                return NotFound();
            }
            var cartItem = await _context.CartItems.Include(ci=>ci.Product).FirstOrDefaultAsync(x=>x.ID==cartItemId);
            if (!cart.Items.Remove(cartItem))
                return NotFound("Cart item not found");
            await hubContext.Clients.All.ItemRemovedFromCart(cartItem.Product); //this will notify the client of the activity
            _context.CartItems.Attach(cartItem);
            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();

            return Accepted();
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.ID == id);
        }

        private async Task<Cart> GetUserCartAsync()
        {
            var userId = await this.GetUserIdAsync(userManager);
            var cart = await _context.Carts.Where(c => c.UserId == userId && c.Status == Status.ACTIVE).
                Include(c => c.Items).
                ThenInclude(i => i.Product).
                FirstOrDefaultAsync();
            if (cart == null)
            {
                var newCart = new Cart { UserId = userId, Status = Status.ACTIVE };
                _context.Carts.Add(newCart);
                await _context.SaveChangesAsync();
                return newCart;
            }
            return cart;
        }
    }
}
