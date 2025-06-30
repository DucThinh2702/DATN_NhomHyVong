using Microsoft.AspNetCore.Mvc;
using DATN.Models;
using System.IO;
using System.Linq;

namespace DATN.Controllers
{
    public class DanhMucController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DanhMucController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /DanhMuc
        public IActionResult Index()
        {
            var list = _context.DanhMucs.ToList();
            return View(list);
        }

        // GET: /DanhMuc/Details/5
        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();

            var danhMuc = _context.DanhMucs.FirstOrDefault(d => d.MaDanhMuc == id);
            if (danhMuc == null)
                return NotFound();

            return View(danhMuc);
        }

        // GET: /DanhMuc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /DanhMuc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DanhMuc danhMuc)
        {
            // Tạo mã tự động
            var lastMa = _context.DanhMucs
                .OrderByDescending(x => x.MaDanhMuc)
                .Select(x => x.MaDanhMuc)
                .FirstOrDefault();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastMa) && lastMa.Length >= 5)
            {
                var numberPart = lastMa.Substring(2);
                if (int.TryParse(numberPart, out int num))
                {
                    nextNumber = num + 1;
                }
            }
            danhMuc.MaDanhMuc = "DM" + nextNumber.ToString("D3");

            // Lưu hình ảnh nếu có
            if (danhMuc.HinhAnhFile != null && danhMuc.HinhAnhFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(danhMuc.HinhAnhFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    danhMuc.HinhAnhFile.CopyTo(stream);
                }

                danhMuc.HinhAnh = "/uploads/" + fileName;
            }

            _context.DanhMucs.Add(danhMuc);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /DanhMuc/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();

            var danhMuc = _context.DanhMucs.Find(id);
            if (danhMuc == null)
                return NotFound();

            return View(danhMuc);
        }

        // POST: /DanhMuc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, DanhMuc danhMuc)
        {
            if (id != danhMuc.MaDanhMuc)
                return NotFound();

            var existing = _context.DanhMucs.Find(id);
            if (existing == null)
                return NotFound();

            existing.TenDanhMuc = danhMuc.TenDanhMuc;
            existing.MoTa = danhMuc.MoTa;

            // Nếu có upload ảnh mới
            if (danhMuc.HinhAnhFile != null && danhMuc.HinhAnhFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                // Xóa file cũ
                if (!string.IsNullOrEmpty(existing.HinhAnh))
                {
                    var oldFile = Path.Combine(_env.WebRootPath, existing.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(oldFile))
                        System.IO.File.Delete(oldFile);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(danhMuc.HinhAnhFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    danhMuc.HinhAnhFile.CopyTo(stream);
                }

                existing.HinhAnh = "/uploads/" + fileName;
            }

            _context.Update(existing);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /DanhMuc/Delete/5
        public IActionResult Delete(string id)
        {
            if (id == null)
                return NotFound();

            var danhMuc = _context.DanhMucs.FirstOrDefault(d => d.MaDanhMuc == id);
            if (danhMuc == null)
                return NotFound();

            return View(danhMuc);
        }

        // POST: /DanhMuc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var danhMuc = _context.DanhMucs.Find(id);
            if (danhMuc != null)
            {
                // Xóa file hình nếu có
                if (!string.IsNullOrEmpty(danhMuc.HinhAnh))
                {
                    var file = Path.Combine(_env.WebRootPath, danhMuc.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(file))
                        System.IO.File.Delete(file);
                }

                _context.DanhMucs.Remove(danhMuc);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
