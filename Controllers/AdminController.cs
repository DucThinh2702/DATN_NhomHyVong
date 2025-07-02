using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Data;
using DATN.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DATN.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> SanPham(string search)
        {
            // Query ban đầu
            var query = _context.Products.AsQueryable();

            // Tìm kiếm nếu có từ khoá
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            // Lấy dữ liệu
            var products = await query.ToListAsync();

            // Tính toán thống kê
            ViewBag.TotalCount = products.Count;
            ViewBag.InStockCount = products.Count(p => p.Stock > 5);
            ViewBag.LowStockCount = products.Count(p => p.Stock > 0 && p.Stock <= 5);
            ViewBag.OutOfStockCount = products.Count(p => p.Stock == 0);

            // Sản phẩm sắp hết hàng
            ViewBag.LowStockProducts = products.Where(p => p.Stock > 0 && p.Stock <= 5).ToList();

            return View(products);
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
        public IActionResult KhachHang()
        {
            return View();
        }
        public IActionResult KhoHang()
        {
            return View();
        }
        public IActionResult DonHang()
        {
            return View();
        }
        public IActionResult CaiDat()
        {
            return View();
        }
    }
}
