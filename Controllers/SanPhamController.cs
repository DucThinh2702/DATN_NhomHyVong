using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using DATN.Models;

namespace DATN.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SanPhamController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: SanPham
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.SanPhams.Include(s => s.DanhMuc).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.TenSP.Contains(search));
            }

            var list = await query.ToListAsync();

            // Thống kê
            ViewBag.TotalCount = list.Count;
            ViewBag.InStockCount = list.Count(s => s.TrangThai == "Đang bán");
            ViewBag.LowStockCount = list.Count(s => s.TrangThai == "Sắp hết");
            ViewBag.OutOfStockCount = list.Count(s => s.TrangThai == "Hết hàng");

            return View(list);
        }

        // GET: SanPham/Details/SP001
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .FirstOrDefaultAsync(m => m.MaSP == id);

            if (sanPham == null)
                return NotFound();

            return View(sanPham);
        }

        // GET: SanPham/Create
        public IActionResult Create()
        {
            ViewBag.DanhMucList = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            return View();
        }

        // POST: SanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenSP,MoTa,GiaBan,GiaGoc,SoLuong,MaDanhMuc")] SanPham sanPham, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucList = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                return View(sanPham);
            }

            // Tạo mã tự động
            var count = await _context.SanPhams.CountAsync();
            sanPham.MaSP = $"SP{(count + 1):D3}";
            sanPham.NgayTao = DateTime.Now;

            // Tự động gán Trạng Thái
            sanPham.TrangThai = sanPham.SoLuong switch
            {
                0 => "Hết hàng",
                <= 5 => "Sắp hết",
                _ => "Đang bán"
            };

            // Upload hình
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "hinh");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                sanPham.HinhAnh = fileName;
            }

            _context.Add(sanPham);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: SanPham/Edit/SP001
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
                return NotFound();

            ViewBag.DanhMucList = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        // POST: SanPham/Edit/SP001
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSP,TenSP,MoTa,GiaBan,GiaGoc,SoLuong,HinhAnh,MaDanhMuc,NgayTao")] SanPham sanPham, IFormFile file)
        {
            if (id != sanPham.MaSP)
                return NotFound();

            var existing = await _context.SanPhams.AsNoTracking().FirstOrDefaultAsync(s => s.MaSP == id);
            if (existing == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucList = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                return View(sanPham);
            }

            // Tự động gán Trạng Thái
            sanPham.TrangThai = sanPham.SoLuong switch
            {
                0 => "Hết hàng",
                <= 5 => "Sắp hết",
                _ => "Đang bán"
            };

            // Upload hình mới nếu có
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "hinh");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Xóa hình cũ
                if (!string.IsNullOrEmpty(existing.HinhAnh))
                {
                    var oldPath = Path.Combine(uploadsFolder, existing.HinhAnh);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                sanPham.HinhAnh = fileName;
            }
            else
            {
                // Giữ hình cũ
                sanPham.HinhAnh = existing.HinhAnh;
            }

            try
            {
                _context.Update(sanPham);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SanPhamExists(sanPham.MaSP))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: SanPham/Delete/SP001
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .FirstOrDefaultAsync(s => s.MaSP == id);

            if (sanPham == null)
                return NotFound();

            return View(sanPham);
        }

        // POST: SanPham/Delete/SP001
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                if (!string.IsNullOrEmpty(sanPham.HinhAnh))
                {
                    var path = Path.Combine(_env.WebRootPath, "hinh", sanPham.HinhAnh);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(string id)
        {
            return _context.SanPhams.Any(e => e.MaSP == id);
        }
    }
}
