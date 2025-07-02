



using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DATN.Data;
using DATN.Models;

namespace DATN.Controllers
{
    public class Products1Controller : Controller
    {
        private readonly AppDbContext _context;

        public Products1Controller(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products1
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            var products = await query.ToListAsync();

            ViewBag.TotalCount = products.Count;
            ViewBag.InStockCount = products.Count(p => p.Stock > 5);
            ViewBag.LowStockCount = products.Count(p => p.Stock > 0 && p.Stock <= 5);
            ViewBag.OutOfStockCount = products.Count(p => p.Stock == 0);
            ViewBag.LowStockProducts = products.Where(p => p.Stock > 0 && p.Stock <= 5).ToList();

            return View(products);
        }

        // GET: Products1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category) // Nếu có navigation property
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products1/Create
        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }

        // POST: Products1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Xử lý ảnh
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/hinh");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    product.ThumbnailImage = "/hinh/" + fileName;
                }

                product.CreatedDate = DateTime.Now;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState lỗi, load lại danh mục
            LoadCategories();
            return View(product);
        }

        // GET: Products1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            LoadCategories();
            return View(product);
        }

        // POST: Products1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile ImageFile)
        {
            if (id != product.ProductID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu có file ảnh mới
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/hinh");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Path.GetFileName(ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        product.ThumbnailImage = "/hinh/" + fileName;
                    }

                    product.UpdatedDate = DateTime.Now;

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            LoadCategories();
            return View(product);
        }

        // GET: Products1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category) // nếu có navigation property
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        /// <summary>
        /// Load danh sách danh mục vào ViewBag.Categories
        /// </summary>
        private void LoadCategories()
        {
            ViewBag.Categories = _context.Categories
                .OrderBy(c => c.CategoryName)
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryID.ToString(),
                    Text = c.CategoryName
                })
                .ToList();
        }
    }
}
