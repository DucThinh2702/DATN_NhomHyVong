using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Data;
using DATN.Models;
using System.Linq;
using System.Threading.Tasks;
using DATN.Models.ViewModels;

namespace DATN.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatnContext _context;

        public AdminController(DatnContext context)
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

        
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var model = new DashboardViewModel
            {
                DoanhThuThangNay = await _context.Orders
                    .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= startOfMonth && o.OrderStatus == "Delivered")
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,

                DonHangMoi = await _context.Orders
                    .CountAsync(o => o.OrderDate.HasValue && o.OrderDate.Value >= DateTime.Today.AddDays(-7)),

                SoKhachHangMoi = await _context.Users
                    .CountAsync(u => u.CreatedDate.HasValue && u.CreatedDate.Value >= DateTime.Today.AddDays(-7)),

                TongSanPhamTrongKho = await _context.Products
                    .SumAsync(p => (int?)p.Stock) ?? 0,

                DonHangGanDay = await _context.Orders
                    .Include(o => o.User)
                    .Where(o => o.OrderDate.HasValue)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .Select(o => new DonHangDto
                    {
                        MaDon = $"ORD-{o.OrderId}",
                        TenKhachHang = o.User.FullName,
                        TongTien = o.TotalAmount ?? 0,
                        TrangThai = o.OrderStatus
                    }).ToListAsync(),

                SanPhamBanChay = await (
    from od in _context.OrderDetails
    join v in _context.ProductVariants on od.VariantId equals v.VariantId
    join p in _context.Products on v.ProductId equals p.ProductId
    join c in _context.Categories on p.CategoryId equals c.CategoryId into cat
    from c in cat.DefaultIfEmpty()
    group od by new { p.ProductName, c.CategoryName } into g
    select new SanPhamBanChayDto
    {
        TenSanPham = g.Key.ProductName,
        DanhMuc = g.Key.CategoryName,
        SoLuongBan = g.Sum(x => (int?)x.Quantity) ?? 0,
        DoanhThu = g.Sum(x => (decimal?)x.TotalPrice) ?? 0
    }
)
.OrderByDescending(x => x.SoLuongBan)
.Take(5)
.ToListAsync(),


                LabelsDoanhThu = Enumerable.Range(0, 7)
                    .Select(i => DateTime.Today.AddDays(-6 + i).ToString("dd/MM"))
                    .ToList()
            };

            // 🛠 Fix chạy tuần tự (KHÔNG dùng Task.WhenAll)
            var dataDoanhThu = new List<decimal>();
            var dataDonHang = new List<int>();

            for (int i = 0; i < 7; i++)
            {
                var date = DateTime.Today.AddDays(-6 + i);

                var total = await _context.Orders
                    .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == date.Date && o.OrderStatus == "Delivered")
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

                var count = await _context.Orders
                    .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == date.Date)
                    .CountAsync();

                dataDoanhThu.Add(total);
                dataDonHang.Add(count);
            }

            model.DataDoanhThu = dataDoanhThu;
            model.DataDonHang = dataDonHang;

            return View(model);
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
        public async Task<IActionResult> MaGiamGia(string status = "Tất cả", string search = "")
        {
            var danhSach = await _context.Promotions.ToListAsync();

            // 1. Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status) && status != "Tất cả")
            {
                danhSach = danhSach.Where(p => p.Status == status).ToList();
            }

            // 2. Lọc theo từ khoá tìm kiếm (mã hoặc tên)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.ToLower();
                danhSach = danhSach.Where(p =>
                    (p.PromoCode.ToString().ToLower().Contains(keyword)) ||
                    (p.PromoName != null && p.PromoName.ToLower().Contains(keyword))
                ).ToList();
            }

            // 3. Tính toán thống kê
            var tongMa = danhSach.Count;
            var daSuDung = danhSach.Sum(p => p.UsedQuantity ?? 0);
            var tongGiamGia = danhSach.Sum(p => (p.UsedQuantity ?? 0) * (p.DiscountValue ?? 0));
            var tongSoLuong = danhSach.Sum(p => p.Quantity ?? 0);
            var tyLeChuyenDoi = tongSoLuong > 0 ? ((double)daSuDung / tongSoLuong * 100).ToString("0.0") : "0";

            // 4. Truyền lên view qua ViewBag
            ViewBag.TongMa = tongMa;
            ViewBag.DaSuDung = daSuDung;
            ViewBag.TietKiem = tongGiamGia;
            ViewBag.ChuyenDoi = tyLeChuyenDoi;
            ViewBag.Search = search;
            ViewBag.CurrentStatus = status;

            return View(danhSach);
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
