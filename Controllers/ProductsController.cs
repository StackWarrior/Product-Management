using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Product_Management.Data;
using Product_Management.Models;
using Product_Management.Models.ViewModels;

namespace Product_Management.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        
        //INDEX
        
        public async Task<IActionResult> Index()
        {
            
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(products);
        }

        
        //DETAILS
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }

        
        //CREATE GET
        
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(new ProductViewModel());
        }

        
        //CREATE POST
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                CategoryId = model.CategoryId,
                CreatedAt = DateTime.Now
            };

            if (model.ImageFile != null)
                product.ImagePath = await SaveImageAsync(model.ImageFile);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Product '{product.Name}' created successfully!";
            return RedirectToAction(nameof(Index));
        }

        
        //EDIT GET
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                ExistingImagePath = product.ImagePath
            };

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(model);
        }

        
        //EDIT POST
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.CategoryId = model.CategoryId;

            if (model.ImageFile != null)
            {
                DeleteImage(product.ImagePath);   
                product.ImagePath = await SaveImageAsync(model.ImageFile); 
            }

            _context.Update(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Product '{product.Name}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        
        //DELETE GET
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }

        
        //DELETE POST
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            DeleteImage(product.ImagePath);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Product '{product.Name}' deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        
        //VALIDATION
        
        [HttpGet]
        public IActionResult IsProductNameAvailable(string name, int id)
        {
            bool exists = _context.Products.Any(p => p.Name == name && p.Id != id);
            return Json(!exists);
        }

        
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return "/images/" + uniqueFileName;
        }

        private void DeleteImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath,
                                           imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}
