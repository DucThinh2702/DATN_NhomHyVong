using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using DATN.Data;
using DATN.Models;
using X.PagedList;

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

        // GET: Category
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? page)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CountSortParm"] = sortOrder == "count" ? "count_desc" : "count";

            var categories = _context.Categories.Include(c => c.Products).AsQueryable();

            ViewBag.TotalCategoryCount = await categories.CountAsync();
            ViewBag.TotalProductCount = await _context.Products.CountAsync();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                categories = categories
                    .Where(c => c.CategoryName.Contains(searchString))
                    .OrderByDescending(c => c.CategoryName.Contains(searchString))
                    .ThenBy(c => c.CategoryName);
            }
            else
            {
                categories = sortOrder switch
                {
                    "name_desc" => categories.OrderByDescending(c => c.CategoryName),
                    "count" => categories.OrderBy(c => c.Products.Count),
                    "count_desc" => categories.OrderByDescending(c => c.Products.Count),
                    _ => categories.OrderBy(c => c.CategoryName)
                };
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;
            return View(await categories.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Create
        public IActionResult Create() => View();

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var uploadPath = Path.Combine(_env.WebRootPath, "hinh", fileName);

                    using var stream = new FileStream(uploadPath, FileMode.Create);
                    await imageFile.CopyToAsync(stream);
                    category.CategoryImage = fileName;
                }

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.FindAsync(id);
            return category == null ? NotFound() : View(category);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile imageFile)
        {
            if (id != category.CategoryId) return NotFound();
            if (!ModelState.IsValid) return View(category);

            try
            {
                var existingCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == id);

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingCategory?.CategoryImage))
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, "hinh", existingCategory.CategoryImage);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var uploadPath = Path.Combine(_env.WebRootPath, "hinh", fileName);

                    using var stream = new FileStream(uploadPath, FileMode.Create);
                    await imageFile.CopyToAsync(stream);

                    category.CategoryImage = fileName;
                }
                else
                {
                    category.CategoryImage = existingCategory?.CategoryImage;
                }

                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(e => e.CategoryId == id))
                    return NotFound();
                throw;
            }
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.CategoryId == id);

            if (category == null) return NotFound();

            if (category.Products.Any())
            {
                TempData["Error"] = "Không thể xóa danh mục vì còn sản phẩm!";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(category.CategoryImage))
            {
                var path = Path.Combine(_env.WebRootPath, "hinh", category.CategoryImage);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
