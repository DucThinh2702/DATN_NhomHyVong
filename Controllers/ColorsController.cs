using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Controllers
{
    public class ColorsController : Controller
    {
        private readonly DatnDbContext _context;

        public ColorsController(DatnDbContext context)
        {
            _context = context;
        }

        // GET: Colors
        public async Task<IActionResult> Index(string? search)
        {
            // Truy vấn cơ bản
            var query = _context.Colors.AsQueryable();

            // Tìm kiếm (nếu có)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.ColorName.Contains(search));
            }

            // Lấy danh sách
            var colors = await query.OrderByDescending(c => c.ColorId).ToListAsync();


            // Đếm tổng số lượng
            ViewBag.TotalColors = await _context.Colors.CountAsync(); // tổng tất cả màu
            ViewBag.FilteredCount = colors.Count;                     // tổng theo tìm kiếm
            ViewBag.Search = search;

            return View(colors);
        }

        // API xóa (AJAX)
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return Json(new { success = false, message = "Không tìm thấy màu" });

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa màu thành công!" });
        }



        // GET: Colors/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Color color, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                bool exists = await _context.Colors
                    .AnyAsync(c => c.ColorName.ToLower() == color.ColorName.ToLower());

                if (exists)
                {
                    TempData["ErrorMessage"] = "Tên màu đã tồn tại!";
                    return RedirectToAction(nameof(Create), new { returnUrl });
                }

                _context.Add(color);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm màu thành công!";

                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction(nameof(Index));
            }
            return View(color);
        }





        // GET: Colors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null) return NotFound();
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return NotFound();
            return View(color);
        }

        // POST: Colors/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Color color)
        {
            if (id != color.ColorId) return NotFound();

            if (ModelState.IsValid)
            {
                // Lấy bản ghi gốc trong DB
                var existingColor = await _context.Colors.AsNoTracking()
                                        .FirstOrDefaultAsync(c => c.ColorId == id);

                if (existingColor == null) return NotFound();

                // Kiểm tra giữ nguyên thông tin
                if (existingColor.ColorName.Trim().ToLower() == color.ColorName.Trim().ToLower())
                {
                    TempData["ErrorMessage"] = "Vui lòng đổi thông tin trước khi lưu!";
                    return RedirectToAction(nameof(Edit), new { id = id });
                }

                // Kiểm tra tên đã tồn tại ở bản ghi khác
                bool exists = await _context.Colors
                    .AnyAsync(c => c.ColorId != id && c.ColorName.ToLower() == color.ColorName.ToLower());

                if (exists)
                {
                    TempData["ErrorMessage"] = "Tên màu đã tồn tại!";
                    return RedirectToAction(nameof(Edit), new { id = id });
                }

                _context.Update(color);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật thông tin màu thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(color); // Trường hợp dữ liệu không hợp lệ
        }



        // GET: Colors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var color = await _context.Colors.FirstOrDefaultAsync(m => m.ColorId == id);
            if (color == null) return NotFound();
            return View(color);
        }

        // POST: Colors/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            // Đặt thông báo
            TempData["SuccessMessage"] = "Xóa màu thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}
