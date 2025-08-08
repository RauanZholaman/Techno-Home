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

        // Displays product list with optional filters
        //GET: Products
        public async Task<IActionResult> Index(List<string> type, List<string> brand, decimal? minPrice, decimal? maxPrice)
        {
            var types = _context.Categories.ToList();   
            var isAdmin = HttpContext.Session.GetString("IsAdmin"); 
    
            ViewBag.Types = types;

            // Base query including Category navigation property for filtering/display
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Filter by CategoryId (as string) if provided and Category exists
            if (type != null && type.Any())
            {
                // Only filter if Category is not null
                query = query.Where(p => p.Category != null && type.Contains(p.Category.CategoryId.ToString()));
            }

            // Apply min/max price range filters if provided
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

        // Show product details by ID        
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

        // Displays product creation form with dynamic dropdowns
        // GET: Products/Create
        public IActionResult Create()
        {
            // Populate category dropdown
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

        // Handles product creation
        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,BrandName,Description,CategoryId,Released,LastUpdated,ImageData,Price")] Product Products, IFormFile image)
        {
            
            try
            {
                // Generate custom sequential ID (not relying on auto-increment)
                Products.Id = _context.Products.Any() ? _context.Products.Max(p => p.Id) + 1 : 1;
                
                // Timestamp and user tracking
                Products.LastUpdated = DateTime.Now;
                
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId.HasValue)
                {
                    Products.LastUpdatedBy = userId.Value;
                }
                
                // Avoid validation of navigation property
                ModelState.Remove(nameof(Products.Category));
                
                
                // Check if image file is attached
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
                
                
                // Convert uploaded image to byte array
                await using (var stream = new MemoryStream())
                {
                    await image.CopyToAsync(stream);
                    Products.ImageData = stream.ToArray();
                }
                
                // Save product to database
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
        
        // Loads edit form with existing product data
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            
            // Map to view model (decouples DB model from form)
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

        // Handles saving changes after product edit
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
                
                // Update all relevant fields
                existingProduct.Name = vm.Name;
                existingProduct.BrandName = vm.BrandName;
                existingProduct.Description = vm.Description;
                existingProduct.CategoryId = vm.CategoryId;
                existingProduct.Released = vm.Released;
                existingProduct.Price = vm.Price;
                existingProduct.LastUpdated = DateTime.Now;
                
                // Track who updated the product
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId.HasValue)
                {
                    existingProduct.LastUpdatedBy = userId.Value;
                }
                
                
                // If a new image is provided, update image data
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
        
        // Show confirmation page before deleting product
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

        // Deletes product from database
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
