using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DATN.Controllers
{
    public class ProductVariantsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductVariantsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ProductVariants
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.ProductVariants
                .Include(v => v.Product)
                .Include(v => v.Color)
                .Include(v => v.Size)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v =>
                    v.Product.ProductName.Contains(search) ||
                    v.Sku.Contains(search));
            }

            var variants = await query
                .OrderBy(v => v.Product.ProductName)
                .ToListAsync();

            ViewBag.TotalVariants = variants.Count;
            ViewBag.ActiveVariants = variants.Count(v => v.Status == "Active");
            ViewBag.LowStockVariants = variants.Count(v => v.Stock > 0 && v.Stock <= 5);
            ViewBag.OutOfStockVariants = variants.Count(v => v.Stock == 0);

            ViewBag.LowStockList = variants
                .Where(v => v.Stock > 0 && v.Stock <= 5)
                .ToList();

            return View(variants);
        }



        // GET: ProductVariants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .Include(v => v.Color)
                .Include(v => v.Size)
                .FirstOrDefaultAsync(m => m.VariantId == id);

            if (variant == null) return NotFound();

            return View(variant);
        }

        // GET: ProductVariants/Create
        public IActionResult Create()
        {
            LoadSelectLists();
            return View();
        }

        // POST: ProductVariants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVariant variant)
        {
            if (ModelState.IsValid)
            {
                variant.CreatedDate = DateTime.Now;
                _context.Add(variant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadSelectLists();
            return View(variant);
        }

        // GET: ProductVariants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null) return NotFound();

            LoadSelectLists();
            return View(variant);
        }

        // POST: ProductVariants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVariant variant, IFormFile ImageFile)
        {
            if (id != variant.VariantId) return NotFound();

            var existingVariant = await _context.ProductVariants.FindAsync(id);
            if (existingVariant == null) return NotFound();

            // Cập nhật các trường
            existingVariant.ProductId = variant.ProductId;
            existingVariant.ColorId = variant.ColorId;
            existingVariant.SizeId = variant.SizeId;
            existingVariant.Sku = variant.Sku;
            existingVariant.Stock = variant.Stock;
            existingVariant.SalePrice = variant.SalePrice;
            existingVariant.OriginalPrice = variant.OriginalPrice;
            existingVariant.Status = variant.Status;
            existingVariant.UpdatedDate = DateTime.Now;

            // Nếu có ảnh mới thì lưu
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/hinh");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                existingVariant.ThumbnailImage = "/hinh/" + fileName;
            }
            else
            {
                // Nếu không có ảnh mới, giữ ảnh cũ từ hidden input
                existingVariant.ThumbnailImage = variant.ThumbnailImage;
            }

            _context.Update(existingVariant);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: ProductVariants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .Include(v => v.Color)
                .Include(v => v.Size)
                .FirstOrDefaultAsync(v => v.VariantId == id);

            if (variant == null) return NotFound();

            return View(variant);
        }

        // POST: ProductVariants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null) return NotFound();

            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xoá biến thể thành công!";
            return RedirectToAction(nameof(Index));
        }


        private void LoadSelectLists()
        {
            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = p.ProductName
                }).ToList();

            ViewBag.Colors = _context.Colors
                .Select(c => new SelectListItem
                {
                    Value = c.ColorId.ToString(),
                    Text = c.ColorName
                }).ToList();

            ViewBag.Sizes = _context.Sizes
                .Select(s => new SelectListItem
                {
                    Value = s.SizeId.ToString(),
                    Text = s.SizeName
                }).ToList();
        }

    }
}
