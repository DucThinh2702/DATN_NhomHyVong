using DATN.Data;
using DATN.IRepository;
using DATN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUsersRepository _context;
        private readonly ILogger<AdminController> _logger;
        public AdminController(ILogger<AdminController> logger, IUsersRepository context)
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
        public async Task<IActionResult> KhachHang(string searchName, string filterBy)
        {
            var listUsers = await _context.GetAllUsers();
            var result = listUsers;

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchName))
            {
                result = result
                    .Where(u => !string.IsNullOrEmpty(u.FullName) &&
                                u.FullName.Contains(searchName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                ViewBag.ListSearchKH = result;
            }

            // Lọc
            if (!string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy.ToLower())
                {
                    case "money":
                        result = result.OrderByDescending(u => u.Orders?.Sum(o => o.TotalAmount) ?? 0).ToList();
                        break;
                    case "date":
                        result = result.OrderByDescending(u => u.CreatedDate).ToList();
                        break;
                }
                ViewBag.ListFilterKH = result;
            }

            // Ưu tiên hiển thị danh sách đã xử lý
            ViewBag.ListCombinedKH = result;

            // Dữ liệu gốc nếu không search/lọc
            if (string.IsNullOrEmpty(searchName) && string.IsNullOrEmpty(filterBy))
            {
                ViewBag.ListKH = listUsers;
            }

            // Gửi lại keyword & filter để giữ giao diện
            ViewBag.SearchKeyword = searchName;
            ViewBag.CurrentFilter = filterBy;

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
