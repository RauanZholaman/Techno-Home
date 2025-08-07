using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Techno_Home.Models;
using StoreDbContext = Techno_Home.Data.StoreDbContext;

namespace Techno_Home.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(StoreDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //GET: Products
        public async Task<IActionResult> Index(List<string> type, List<string> brand, decimal? minPrice, decimal? maxPrice)
        {
            var types = _context.Categories.ToList();
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
    
            ViewBag.Types = types;

            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (type != null && type.Any())
            {
                // Only filter if Category is not null
                query = query.Where(p => p.Category != null && type.Contains(p.Category.CategoryId.ToString()));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price != null && p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price != null && p.Price <= maxPrice.Value);
            }

            return View(await query.ToListAsync());
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products 
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name");
            try
            {
                ViewBag.Users = new SelectList(_context.Users, "Name", "Name");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
                ViewBag.Users = new List<SelectListItem>(); // Empty list to avoid crash
            }
            return View();
        }

        // POST: Products/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,BrandName,Description,CategoryId,SubCategoryId,Released,LastUpdated,ImageData,Price")] Product Products, IFormFile image)
        {
            
            try
            {
                // Fix the Id generation
                Products.Id = _context.Products.Any() ? _context.Products.Max(p => p.Id) + 1 : 1;
                
                // Set the timestamp
                Products.LastUpdated = DateTime.Now;
                
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId.HasValue)
                {
                    Products.LastUpdatedBy = userId.Value;
                }
                
                ModelState.Remove(nameof(Products.Category));
                
                if (image == null || image.Length == 0)
                {
                    ModelState.AddModelError(nameof(image), "Please choose an image");
                }
                
                // Check if ModelState is valid and show errors on the page
                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "Form validation failed. Please check all fields.";
                    
                    // Add all validation errors to ViewBag for debugging
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    ViewBag.ValidationErrors = string.Join(", ", errors);
                    
                    return View(Products);
                }

                await using (var stream = new MemoryStream())
                {
                    await image.CopyToAsync(stream);
                    Products.ImageData = stream.ToArray();
                }

                _context.Add(Products);
                
                var result = await _context.SaveChangesAsync();
                
                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ErrorMessage = "No records were saved to the database.";
                    return View(Products);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                ViewBag.ExceptionDetails = ex.ToString();
                return View(Products);
            }
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            
            var viewModel = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                BrandName = product.BrandName,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Released = product.Released,
                Price = product.Price,
            };
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel vm, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null) return NotFound();

                if (!ModelState.IsValid)
                    return View(vm);

                // Update fields
                existingProduct.Name = vm.Name;
                existingProduct.BrandName = vm.BrandName;
                existingProduct.Description = vm.Description;
                existingProduct.CategoryId = vm.CategoryId;
                existingProduct.Released = vm.Released;
                existingProduct.Price = vm.Price;

                existingProduct.LastUpdated = DateTime.Now;
                
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId.HasValue)
                {
                    existingProduct.LastUpdatedBy = userId.Value;
                }
                
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await ImageFile.CopyToAsync(ms);
                    existingProduct.ImageData = ms.ToArray();
                }
            
                _context.Update(existingProduct);
                await _context.SaveChangesAsync();   
            }

            return RedirectToAction(nameof(Index));
        }
        

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Products == null)
            {
                return NotFound();
            }

            return View(Products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Products = await _context.Products.FindAsync(id);
            if (Products != null)
            {
                _context.Products.Remove(Products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        
    }
}
