using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using petshop.Data;
using petshop.Models;
using petshop.ViewModels;

namespace petshop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyDbContext _context;

        public ProductsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            //return View(products) don't need this view to dispay for this prac;
            //for test propose only, to practice how to use linq query and pass data to view page
            //var productsName = _context.Products.Where(x => x.Price > 90).Select(x => x.Name).ToList();
            //var products = _context.Products.ToList();
            //ViewBag.ProductName = productsName;
            return RedirectToAction("Index", "Home");
        }

 

        // GET: Products/Product/5
        public async Task<IActionResult> Product(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products              
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var relatedProducts = await _context.Products
            .Where(p => p.Category == product.Category && p.Id != product.Id)
             .ToListAsync();

            var viewModel = new ProductViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts
            };
            return View(viewModel);
        }

        private bool ProductsExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: Products/Search
        public IActionResult Search(string searchString)
        {
            var products = _context.Products.Where(p => p.Name.Contains(searchString)).ToList();
            ViewBag.SearchString = searchString;
            return View(products);
        }

    }
}
