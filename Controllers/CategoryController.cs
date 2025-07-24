using Microsoft.AspNetCore.Mvc;
using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace DATN.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DatnContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(DatnContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Category
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var categories = _context.Categories
                .Include(c => c.Products)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c => c.CategoryName.Contains(searchString));
            }

            var categoryList = await categories.ToListAsync();
            return View(categoryList);
        }

        // GET: /Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(category);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uniqueFileName = $"{Path.GetFileNameWithoutExtension(imageFile.FileName)}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var uploadPath = Path.Combine(_env.WebRootPath, "hinh");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                category.CategoryImage = uniqueFileName;
            }

            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi thêm danh mục: " + ex.Message;
                return View(category);
            }
        }

        // GET: /Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile imageFile)
        {
            if (id != category.CategoryId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Dữ liệu không hợp lệ.";
                return View(category);
            }

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = $"{Path.GetFileNameWithoutExtension(imageFile.FileName)}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                    var uploadPath = Path.Combine(_env.WebRootPath, "hinh");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    category.CategoryImage = uniqueFileName;
                }

                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId))
                    return NotFound();
                else
                    throw;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi cập nhật: " + ex.Message;
                return View(category);
            }
        }

        // GET: /Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                if (!string.IsNullOrEmpty(category.CategoryImage))
                {
                    var path = Path.Combine(_env.WebRootPath, "hinh", category.CategoryImage);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
