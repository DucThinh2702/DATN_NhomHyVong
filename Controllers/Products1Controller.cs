using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DATN.Controllers
{
    public class Products1Controller : Controller
    {
        private readonly DatnDbContext _context;

        public Products1Controller(DatnDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, string startsWith, int page = 1)
        {
            int pageSize = 6; // 6 sản phẩm / trang

            var query = _context.Products.AsQueryable();

            // Lọc theo ký tự đầu (nếu có)
            if (!string.IsNullOrEmpty(startsWith))
            {
                query = query.Where(p => p.ProductName.StartsWith(startsWith));
            }

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            // Tổng sản phẩm sau lọc
            var totalItems = await query.CountAsync();

            // Lấy danh sách sản phẩm theo trang
            var data = await query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ThumbnailImage = p.ThumbnailImage,
                    SalePrice = p.SalePrice,
                    CreatedDate = p.CreatedDate,
                    Stock = _context.ProductVariants
                                    .Where(v => v.ProductId == p.ProductId)
                                    .Sum(v => (int?)v.Stock) ?? 0
                })
                .ToListAsync();

            // Tính tổng tồn kho để hiển thị thống kê
            ViewBag.TotalCount = totalItems;
            ViewBag.InStockCount = data.Count(p => p.Stock > 5);
            ViewBag.LowStockCount = data.Count(p => p.Stock > 0 && p.Stock <= 5);
            ViewBag.OutOfStockCount = data.Count(p => p.Stock == 0);
            ViewBag.LowStockProducts = data.Where(p => p.Stock > 0 && p.Stock <= 5).ToList();

            // Truyền thông tin phân trang cho View
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(data);
        }




        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.Size)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }



        // GET: Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
     Product product,
     [Bind(Prefix = "variants")] List<ProductVariant> variants,
     IFormFile ImageFile)
        {
            if (variants == null || !variants.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một biến thể.";
                LoadDropdowns();
                return View(product);
            }

            // Lưu ảnh đại diện sản phẩm
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var productFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/hinh");
                Directory.CreateDirectory(productFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(productFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                product.ThumbnailImage = "/hinh/" + fileName;
            }

            product.CreatedDate = DateTime.Now;
            product.UpdatedDate = DateTime.Now;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            foreach (var variant in variants)
            {
                variant.ProductId = product.ProductId;
                variant.Sku = $"BAG-{product.ProductId}-{variant.ColorId}-{variant.SizeId}";
                variant.CreatedDate = DateTime.Now;
                variant.UpdatedDate = DateTime.Now;
                variant.Status = "Active";
                variant.SalePrice = product.SalePrice;
                variant.OriginalPrice = product.OriginalPrice;

                if (variant.ImageFile != null && variant.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/hinh");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(variant.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await variant.ImageFile.CopyToAsync(stream);
                    }

                    variant.ThumbnailImage = "/hinh/" + fileName;
                }

                _context.ProductVariants.Add(variant);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
            return RedirectToAction(nameof(Index));

        }





        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // Lấy danh mục
            ViewBag.Categories = new SelectList(
                _context.Categories.OrderBy(c => c.CategoryName),
                "CategoryId",
                "CategoryName",
                product.CategoryId
            );

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile ImageFile)
        {
            if (id != product.ProductId) return NotFound();

            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật các trường được chỉnh sửa
                    existing.ProductName = product.ProductName;
                    existing.Description = product.Description;
                    existing.SalePrice = product.SalePrice;
                    existing.OriginalPrice = product.OriginalPrice;
                    existing.CategoryId = product.CategoryId;
                    existing.Size = product.Size;
                    existing.Color = product.Color;
                    existing.Material = product.Material;
                    existing.Status = product.Status;
                    existing.UpdatedDate = DateTime.Now;

                    // Nếu có upload ảnh mới
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

                        existing.ThumbnailImage = "/hinh/" + fileName;
                    }
                    // Nếu không upload thì giữ nguyên existing.ThumbnailImage

                    _context.Update(existing);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId)) return NotFound();
                    else throw;
                }
            }

            // Nếu ModelState invalid → trả về existing để hiển thị ảnh cũ
            return View(existing);
        }




        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }


        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var variants = _context.ProductVariants.Where(v => v.ProductId == id);
            _context.ProductVariants.RemoveRange(variants);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // --- Thêm dòng này để hiển thị thông báo sau khi xóa ---
            TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";

            return RedirectToAction(nameof(Index));
        }


        //private bool ProductExists(int id)
        //{
        //    return _context.Products.Any(e => e.ProductId == id);
        //}

        private void LoadDropdowns()
        {
            ViewBag.Categories = new SelectList(_context.Categories.OrderBy(c => c.CategoryName), "CategoryId", "CategoryName");
            ViewBag.Colors = _context.Colors.OrderBy(c => c.ColorName).ToList();
            ViewBag.Sizes = _context.Sizes.OrderBy(s => s.SizeName).ToList();
        }
    }
}
