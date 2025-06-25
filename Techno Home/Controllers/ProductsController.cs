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
        private readonly Techno_HomeContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(Techno_HomeContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price")] Product product, IFormFile image)
        {
            ModelState.Remove(nameof(product.ImageFileName));
            
            if (image == null || image.Length == 0)
            {
                ModelState.AddModelError(nameof(image), "Please choose an image");
            }
            
            if (!ModelState.IsValid)
                return View(product);
            
            // if (ModelState.IsValid) // The original condition
            // {
            //     _context.Add(product);
            //     await _context.SaveChangesAsync();
            //     return RedirectToAction(nameof(Index));
            // }
            // return View(product);
            
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            
            var uploadDir = Path.Combine(_env.WebRootPath, "images", "products");
            Directory.CreateDirectory(uploadDir);
            var savePath = Path.Combine(uploadDir, fileName);

            await using (var fs = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fs);
            }

            product.ImageFileName = fileName;

            _context.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id, IFormFile? image)
        {
            
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            
            if (id == null)
            {
                return NotFound();
            }
            
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price")] Product product, IFormFile? image)
        {
            var existingProduct = await _context.Product.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(product.ImageFileName));
            
            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var uploadDir = Path.Combine(_env.WebRootPath!, "images", "products");
                Directory.CreateDirectory(uploadDir);
                var savePath = Path.Combine(uploadDir, fileName);
            
                await using var fs = new FileStream(savePath, FileMode.Create);
                await image.CopyToAsync(fs);
            
                existingProduct.ImageFileName = fileName;
            }
            
            /////////////////////////////////////////// Original Implementation 
            
            if (id != product.Id)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                // try
                // {
                //     /* Save to DB */
                //     _context.Update(product);
                //     await _context.SaveChangesAsync();
                // }
                // catch (DbUpdateConcurrencyException)
                // {
                //     if (!ProductExists(product.Id))
                //     {
                //         return NotFound();
                //     }
                //     else
                //     {
                //         throw;
                //     }
                // }
                // return RedirectToAction(nameof(Index));
                return View(product);
            }
            
            existingProduct.Title       = product.Title;
            existingProduct.ReleaseDate = product.ReleaseDate;
            existingProduct.Genre       = product.Genre;
            existingProduct.Price       = product.Price;
            
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

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
