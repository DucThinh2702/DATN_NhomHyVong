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
            // Truy vấn ban đầu
            var query = _context.Products.AsQueryable();

            // Tìm kiếm nếu có từ khoá
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            // Lấy dữ liệu về
            var products = await query.ToListAsync();

            // Tính toán số lượng
            ViewBag.TotalCount = products.Count;
            ViewBag.InStockCount = products.Count(p => p.Stock > 5);
            ViewBag.LowStockCount = products.Count(p => p.Stock > 0 && p.Stock <= 5);
            ViewBag.OutOfStockCount = products.Count(p => p.Stock == 0);

            // Danh sách sản phẩm sắp hết hàng (Stock <=5)
            ViewBag.LowStockProducts = products.Where(p => p.Stock > 0 && p.Stock <= 5).ToList();

            return View(products);
        }



        // GET: Products1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products1/Create
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        {
            // Tạo thư mục nếu chưa có
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/hinh");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Nếu có file ảnh
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // Lưu đường dẫn
                product.ThumbnailImage = "/hinh/" + fileName;
            }

            // Gán ngày tạo
            product.CreatedDate = DateTime.Now;

            // Lưu vào database
            _context.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // có model danh mục  mở này ra xóa trên
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        //{
        //    // Validate ModelState trước
        //    if (ModelState.IsValid)
        //    {
        //        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/hinh");
        //        if (!Directory.Exists(uploadsFolder))
        //        {
        //            Directory.CreateDirectory(uploadsFolder);
        //        }

        //        if (ImageFile != null && ImageFile.Length > 0)
        //        {
        //            var fileName = Path.GetFileName(ImageFile.FileName);
        //            var filePath = Path.Combine(uploadsFolder, fileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await ImageFile.CopyToAsync(stream);
        //            }

        //            product.ThumbnailImage = "/hinh/" + fileName;
        //        }

        //        product.CreatedDate = DateTime.Now;

        //        _context.Add(product);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewBag.Categories = _context.Categories
        //        .OrderBy(c => c.CategoryName)
        //        .Select(c => new SelectListItem
        //        {
        //            Value = c.CategoryID.ToString(),
        //            Text = c.CategoryName
        //        })
        //        .ToList();

        //    return View(product);
        //}

        // GET: Products1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,Description,SalePrice,OriginalPrice,Stock,Size,Color,Material,CategoryID,CreatedDate,ThumbnailImage,Status")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.UpdatedDate = DateTime.Now;

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
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
            return View(product);
        }


        // GET: Products1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
