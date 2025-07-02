using DATN.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controllers
{
    public class AdminController : Controller
    {
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
        private readonly OrderRepository _orderRepo;

        public AdminController(OrderRepository orderRepo) // Inject repository
        {
            _orderRepo = orderRepo;
        }

        public async Task<IActionResult> DonHang()
        {
            var orders = await _orderRepo.GetAllAsync();
            return View(orders); // Truyền danh sách đơn hàng sang view
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
