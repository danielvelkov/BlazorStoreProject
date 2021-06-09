using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPProject.Server.Controllers.HelperClasses;
using PPProject.Server.Data;
using PPProject.Server.Models;
using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment env;

        public ProductsController(ApplicationDbContext context,
            UserManager<ApplicationUser> manager,
            IWebHostEnvironment environment)
        {
            _context = context;
            userManager = manager;
            env = environment;
        }

        // GET: api/Products
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products =  _context.Products.Include(p => p.Category).
                Include(p => p.ProductPic).ToList();

            FileImageHandler handler = new(env);
            foreach (var product in products)
            {
                product.ProductPic = await handler.GetImageAsync(product.ProductPic?.FileName);
            }
            return products;
        }

        [HttpGet("suggestions")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsThatUserMightLike()
        {
            string userId = await this.GetUserIdAsync(userManager);
            var userCarts = _context.Carts.Include(c=>c.Items).Where(c => c.UserId == userId);
            var suggestedProducts = new List<Product>();
            if (userCarts != null)
            {
               suggestedProducts = await _context.CartItems.
               Join(userCarts, ci => ci.CartId, c => c.ID, (ci, c) => ci.ProductId).
               Join(_context.Products.Include(p => p.ProductPic), id => id, p => p.ID, (id, p) => p).ToListAsync();

                FileImageHandler handler = new(env);
                foreach (var product in suggestedProducts)
                {
                    product.ProductPic = await handler.GetImageAsync(product.ProductPic?.FileName);
                }
            }

            return suggestedProducts;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.Include(i => i.Category).Include(i =>i.ProductPic)
                .FirstOrDefaultAsync(i => i.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            FileImageHandler handler = new(env);
            product.ProductPic = await handler.GetImageAsync(product.ProductPic?.FileName);

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ID)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                //update any cart items that might have this item
                await _context.CartItems.Where(c => c.ProductId == id).
                    ForEachAsync(i => i.Discount = product.Discount);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<int>> PostProduct(Product product)
        {
            if (product == null)
            {
                BadRequest("Something went wrong on your end");
            }

            if (product.ProductPic != null)
            {
                FileImageHandler handler = new(env);
                var productPic = new FileImage
                {
                    FileName = await handler.SaveImageAsync(product.ProductPic)
                };
                product.ProductPic = productPic;
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product.ID;
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.Include(p=>p.ProductPic)
                .FirstOrDefaultAsync(p=>p.ID==id);
            if (product == null)
            {
                return NotFound();
            }

            FileImageHandler handler = new(env);


            bool imageDeleted = handler.DeleteImage(product.ProductPic);
            if (!imageDeleted)
                return BadRequest("Something is wrong with the server");

            var relatedCartItems = _context.CartItems.Where(ci => ci.ProductId == id);
            _context.CartItems.RemoveRange(relatedCartItems);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }
    }
}
