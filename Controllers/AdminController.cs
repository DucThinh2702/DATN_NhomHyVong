using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PhanTich()
        {
            return View();
        }

        public IActionResult ChienDich()
        {
            return View();
        }
        public IActionResult DanhMuc()
        {
            return View();
        }
        public IActionResult MaGiamGia()
        {
            return View();
        }
        [HttpGet]
        public IActionResult KhachHang()
        {
            var listUsers = _context.Users
                .ToList();

            ViewBag.ListKH = listUsers;
            return View(listUsers); // Không cần truyền model
        }
        public IActionResult KhoHang()
        {
            return View();
        }
        public IActionResult DonHang()
        {
            return View();
        }
        public IActionResult SanPham()
        {
            return View();
        }
        public IActionResult CaiDat()
        {
            return View();
        }
    }
}
