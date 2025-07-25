using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Techno_Home.Data;
using Techno_Home.Models;

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

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
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
                ViewBag.Users = new SelectList(_context.Users, "UserName", "UserName");
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
        public async Task<IActionResult> Create([Bind("Id,Name,BrandName,Description,CategoryId,SubCategoryId,Released,LastUpdatedBy,LastUpdated,ImagePath,Price")] Product Products, IFormFile image)
        {
            try
            {
                // Fix the Id generation
                Products.Id = _context.Products.Any() ? _context.Products.Max(p => p.Id) + 1 : 1;
                
                // Set the timestamp
                Products.LastUpdated = DateTime.Now;
                
                ModelState.Remove(nameof(Products.ImagePath));
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
                
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                
                var uploadDir = Path.Combine(_env.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadDir);
                var savePath = Path.Combine(uploadDir, fileName);

                await using (var fs = new FileStream(savePath, FileMode.Create))
                {
                    await image.CopyToAsync(fs);
                }

                Products.ImagePath = fileName;

                _context.Add(Products);
                
                // Add this to see if SaveChanges actually gets called
                ViewBag.DebugMessage = "About to save to database...";
                
                var result = await _context.SaveChangesAsync();
                
                // Check how many records were affected
                ViewBag.DebugMessage = $"SaveChanges returned: {result} records affected.";
                
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

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id, IFormFile? image)
        {
            var Products = await _context.Products.FindAsync(id);
            if (Products == null)
            {
                return NotFound();
            }
            
            if (id == null)
            {
                return NotFound();
            }
            
            return View(Products);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Released,CategoryId,Price")] Product Products, IFormFile? image)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Products.ImagePath));
            
            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var uploadDir = Path.Combine(_env.WebRootPath!, "images", "products");
                Directory.CreateDirectory(uploadDir);
                var savePath = Path.Combine(uploadDir, fileName);
            
                await using var fs = new FileStream(savePath, FileMode.Create);
                await image.CopyToAsync(fs);
            
                existingProduct.ImagePath = fileName;
            }
            
            if (id != Products.Id)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                return View(Products);
            } 
            
            existingProduct.Name       = Products.Name;
            existingProduct.Released   = Products.Released;
            existingProduct.CategoryId = Products.CategoryId;
            existingProduct.Price      = Products.Price;
            
            _context.Update(existingProduct);
            await _context.SaveChangesAsync();
            
            
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
