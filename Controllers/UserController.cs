using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Data;
using System.Linq;
using DATN.Models;

namespace DATN.Controllers
{
    public class UserController : Controller
    {
        private readonly DatnDbContext _context;

        public UserController(DatnDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, string search = "")
        {
            int pageSize = 8;

            // Lấy danh mục
            var categories = _context.Categories.ToList();

            // Lấy danh sách sản phẩm (lọc trước khi phân trang)
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            var totalProducts = query.Count();
            var products = query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Lấy 4 sản phẩm mới nhất (section riêng)
            var top4Products = _context.Products
                .OrderByDescending(p => p.CreatedDate)
                .Take(4)
                .ToList();

            // Truyền dữ liệu sang View
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            ViewBag.Categories = categories;
            ViewBag.Search = search;
            ViewBag.Top4Products = top4Products; // 4 sản phẩm mới nhất

            return View(products); // danh sách chính có phân trang
        }

        public IActionResult GioHang() => View();
        //public IActionResult ThanhToan() => View();
        public IActionResult LienHe() => View();
        public IActionResult QuenMatKhau() => View();
        public IActionResult DangNhap() => View();
        [HttpPost]
        public IActionResult DangNhap(string username, string password)
        {
            // Tìm user trong DB
            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View();
            }

            // Lưu session UserID
            HttpContext.Session.SetInt32("UserID", user.UserId);

            // ➜ Chuyển đến trang đơn hàng
            return RedirectToAction("ThanhToan", "User");
        }

        public IActionResult DangKy() => View();
        public IActionResult ChiTiet() => View();
        public IActionResult ThanhToan()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("DangNhap");
            }

            var cart = _context.Carts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Variant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                ViewBag.CartItems = new List<object>();
                ViewBag.Total = 0;
                ViewBag.ShippingFee = 0;
                return View();
            }

            var items = cart.CartDetails.Select(cd => new
            {
                ProductName = cd.Variant.Product.ProductName,
                Quantity = cd.Quantity ?? 0,
                UnitPrice = cd.Variant.SalePrice ?? 0,
                TotalPrice = (cd.Quantity ?? 0) * (cd.Variant.SalePrice ?? 0)
            }).ToList();

            int totalQuantity = items.Sum(i => i.Quantity);
            decimal total = items.Sum(i => i.TotalPrice);
            decimal shippingFee = 30000 + (totalQuantity - 1) * 15000;

            ViewBag.CartItems = items;
            ViewBag.Total = total;
            ViewBag.ShippingFee = shippingFee;

            return View();
        }


        [HttpPost]
        public IActionResult ThanhToan(string recipientName, string recipientPhone, string deliveryAddress, int paymentMethodId)
        {
            // 1. Lấy UserID đang đăng nhập
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return Json(new { success = false, message = "Bạn chưa đăng nhập!" });

            // 2. Lấy giỏ hàng
            var cart = _context.Carts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Variant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefault(c => c.UserId == userId);  // DÙNG UserId (đúng với model)

            if (cart == null || !cart.CartDetails.Any())
                return Json(new { success = false, message = "Giỏ hàng trống!" });

            // 3. Tính phí ship
            int totalQuantity = cart.CartDetails.Sum(x => x.Quantity ?? 0);
            decimal productTotal = cart.CartDetails.Sum(x => (x.Quantity ?? 0) * (x.Variant.SalePrice ?? 0));
            decimal shippingFee = 30000 + (totalQuantity - 1) * 15000;

            // 4. Tạo đơn hàng
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Quantity = totalQuantity,
                TotalAmount = productTotal + shippingFee,
                PaymentMethodId = paymentMethodId,
                PaymentStatus = "Chưa thanh toán",
                OrderStatus = "Chờ xử lý",
                RecipientName = recipientName,
                RecipientPhone = recipientPhone,
                DeliveryAddress = deliveryAddress,
                Note = "",
                ShippingFee = shippingFee
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            // 5. Chi tiết đơn hàng & cập nhật tồn kho
            foreach (var item in cart.CartDetails)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    VariantId = item.VariantId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Variant.SalePrice,
                    TotalPrice = (item.Quantity ?? 0) * (item.Variant.SalePrice ?? 0)
                });

                if (item.Variant.Stock.HasValue)
                    item.Variant.Stock -= item.Quantity ?? 0;
            }
            _context.SaveChanges();

            // 6. Xóa giỏ hàng
            _context.CartDetails.RemoveRange(cart.CartDetails);
            _context.SaveChanges();

            // 7. Trả kết quả
            var orderData = new
            {
                order.OrderId,
                order.OrderDate,
                order.TotalAmount,
                order.ShippingFee,
                order.OrderStatus,
                Items = _context.OrderDetails
                        .Include(od => od.Variant)
                            .ThenInclude(v => v.Product)
                        .Where(od => od.OrderId == order.OrderId)
                        .Select(od => new
                        {
                            od.Variant.Product.ProductName,
                            od.Quantity,
                            od.TotalPrice
                        }).ToList()
            };

            return Json(new { success = true, order = orderData });
        }

    }
}
