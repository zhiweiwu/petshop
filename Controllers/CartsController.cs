
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using petshop.Data;
using petshop.Models;

namespace petshop.Controllers
{
    public class CartsController : Controller
    {
        private readonly MyDbContext _context;

        public CartsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Products)
                .FirstOrDefaultAsync(c => c.UserId == userId);
 
            return View(cart);
        }
       
        [HttpPost]
        // Post:Carts/AddToCart/ProductId
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {                 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId==null)
            {   //if is productive environment change host to tony-petshop.azurewebsites.net 
                string host = HttpContext.Request.Host.ToUriComponent();
                var url = Url.Page("/Account/Login", null, new { area = "Identity" }, "https", host);
                return Redirect(url);
            }

            // retrieve the cart data from the database, including its associated cart items and products. 
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Products)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var product = await _context.Products.FindAsync(productId);
            //create a cart instance if user didnt buy any product, if is first time add product, increase 1,
            //otherwise +1
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                cart.CartItems = new List<CartItem> { new CartItem { Products = product, Quantity = quantity } };
                _context.Carts.Add(cart);
            }
            else
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Products.Id == productId);
                if (cartItem == null)
                {
                    cart.CartItems.Add(new CartItem { Products = product, Quantity = quantity });
                }
                else
                {
                    cartItem.Quantity += quantity;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Carts", new { area = "" });

        }
        [HttpPost]
        public async Task<IActionResult> UpdateCart(int cartItemId, int quantity)
        {
            var cartItem = _context.CartItems.FirstOrDefault(x => x.Id == cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Carts", new { area = "" });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(x => x.Id == cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Carts");
        }
    }
}
