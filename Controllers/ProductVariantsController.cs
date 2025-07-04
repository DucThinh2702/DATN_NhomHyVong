using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DATN.Controllers
{
    public class ProductVariantsController : Controller
    {
        private readonly DatnContext _context;

        public ProductVariantsController(DatnContext context)
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
                    v.SKU.Contains(search));
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
        public async Task<IActionResult> Edit(int id, ProductVariant variant)
        {
            if (id != variant.VariantId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy bản ghi gốc từ DB
                    var existingVariant = await _context.ProductVariants.FindAsync(id);
                    if (existingVariant == null) return NotFound();

                    // Cập nhật các trường
                    existingVariant.ProductId = variant.ProductId;
                    existingVariant.ColorId = variant.ColorId;
                    existingVariant.SizeId = variant.SizeId;
                    existingVariant.SKU = variant.SKU;
                    existingVariant.Stock = variant.Stock;
                    existingVariant.SalePrice = variant.SalePrice;
                    existingVariant.OriginalPrice = variant.OriginalPrice;
                    existingVariant.ThumbnailImage = variant.ThumbnailImage;
                    existingVariant.Status = variant.Status;

                    // Chỉ gán UpdatedDate
                    existingVariant.UpdatedDate = DateTime.Now;

                    _context.Update(existingVariant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VariantExists(variant.VariantId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            LoadSelectLists();
            return View(variant);
        }


        // GET: ProductVariants/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: ProductVariants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant != null)
            {
                _context.ProductVariants.Remove(variant);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VariantExists(int id)
        {
            return _context.ProductVariants.Any(e => e.VariantId == id);
        }

        /// <summary>
        /// Nạp danh sách dropdown Product, Color, Size
        /// </summary>
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
