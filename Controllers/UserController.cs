using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Data;
using System.Linq;
using DATN.Models;
using Newtonsoft.Json;
using static NuGet.Packaging.PackagingConstants;

namespace DATN.Controllers
{
    public class UserController : Controller
    {
        private readonly DatnDbContext _context;

        public UserController(DatnDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, string search = "", int? categoryId = null)
        {
            int pageSize = 8;

            // Lấy danh mục để hiển thị trên menu
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;

            // Query sản phẩm
            var query = _context.Products.AsQueryable();

            // Lọc theo tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
                


            // Tổng sản phẩm để phân trang
            var totalProducts = query.Count();

            // Sản phẩm theo trang
            var products = query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 4 sản phẩm mới nhất cho phần "Sản phẩm gần đây"
            var top4Products = _context.Products
                .OrderByDescending(p => p.CreatedDate)
                .Take(4)
                .ToList();
            ViewBag.Top4Products = top4Products;

            // Gửi thông tin phân trang + lọc sang View
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.SelectedCategoryId = categoryId;

            return View(products);
        }
    


        [HttpPost]
        public IActionResult ThanhToan(string recipientName, string recipientPhone, string deliveryAddress, int paymentMethodId)
        {
            var userId = HttpContext.Session.GetInt32("UserID") ?? 2;

            var cart = _context.Carts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Variant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.CartDetails.Any())
                return Json(new { success = false, message = "Giỏ hàng trống!" });

            int totalQuantity = cart.CartDetails.Sum(x => x.Quantity ?? 0);
            decimal productTotal = cart.CartDetails.Sum(x => (x.Quantity ?? 0) * (x.Variant.SalePrice ?? 0));
            decimal shippingFee = 30000 + (totalQuantity - 1) * 15000;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Quantity = totalQuantity,
                TotalAmount = productTotal + shippingFee,
                PaymentMethodId = paymentMethodId,
                PaymentStatus = paymentMethodId == 2 ? "Chờ chuyển khoản" : "Chưa thanh toán",
                OrderStatus = "Chờ xử lý",
                RecipientName = recipientName,
                RecipientPhone = recipientPhone,
                DeliveryAddress = deliveryAddress,
                Note = "",
                ShippingFee = shippingFee
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

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
            _context.CartDetails.RemoveRange(cart.CartDetails);
            _context.SaveChanges();

            // Tạo QR link động (MBbank)
            string content = $"TTDH{order.OrderId}";
            string qrUrl = $"https://img.vietqr.io/image/MB-0836641809-qr_only.png?amount={(int)order.TotalAmount}&addInfo={content}";

            return Json(new
            {
                success = true,
                order = new
                {
                    order.OrderId,
                    order.OrderDate,
                    order.TotalAmount,
                    order.ShippingFee,
                    order.OrderStatus,
                    order.PaymentStatus,
                    order.PaymentMethodId
                },
                bankInfo = paymentMethodId == 2 ? new
                {
                    BankName = "MB Bank",
                    AccountName = "Nguyễn Đức Thịnh",
                    AccountNumber = "0836641809",
                    Content = content,
                    QR = qrUrl
                } : null
            });
        }
        public IActionResult GioHang()
        {
            var userId = HttpContext.Session.GetInt32("UserID") ?? 2;

            var cart = _context.Carts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Variant)
                        .ThenInclude(v => v.Product)
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Variant.Color)
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Variant.Size)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
                return RedirectToAction("Index", "User");

            return View(cart); // Model là Cart
        }



        public IActionResult ChonBienThe(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();

            var variants = _context.ProductVariants
                .Include(v => v.Color)
                .Include(v => v.Size)
                .Where(v => v.ProductId == id && v.Status == "Active")
                .ToList();

            // lấy màu và size duy nhất
            ViewBag.Colors = variants.Select(v => v.Color).Distinct().ToList();
            ViewBag.Sizes = variants.Select(v => v.Size).Distinct().ToList();

            // serialize data tránh vòng lặp
            var variantDtos = variants.Select(v => new
            {
                v.VariantId,
                v.ColorId,
                v.SizeId,
                v.SalePrice,
                v.Stock,
                v.ThumbnailImage
            }).ToList();
            ViewBag.VariantsJson = JsonConvert.SerializeObject(variantDtos);

            return View(product);
        }

        [HttpPost]
        public IActionResult ThemVaoGio(int variantId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserID") ?? 2;

            var cart = _context.Carts.Include(c => c.CartDetails)
                                     .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, CreatedDate = DateTime.Now, LastUpdated = DateTime.Now };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            var existingItem = cart.CartDetails.FirstOrDefault(c => c.VariantId == variantId);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                _context.CartDetails.Add(new CartDetail { CartId = cart.CartId, VariantId = variantId, Quantity = quantity });

            cart.LastUpdated = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public IActionResult MuaNgay(int variantId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserID") ?? 2;

            var cart = _context.Carts
                               .Include(c => c.CartDetails)
                               .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedDate = DateTime.Now,
                    LastUpdated = DateTime.Now
                };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            var existingItem = cart.CartDetails.FirstOrDefault(c => c.VariantId == variantId);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                _context.CartDetails.Add(new CartDetail
                {
                    CartId = cart.CartId,
                    VariantId = variantId,
                    Quantity = quantity
                });

            cart.LastUpdated = DateTime.Now;
            _context.SaveChanges();

            // ➡️ Chuyển đến giỏ hàng
            return RedirectToAction("GioHang", "User", new { id = cart.CartId });
        }
        [HttpPost]
        public IActionResult XoaKhoiGio(int cartDetailId)
        {
            var detail = _context.CartDetails.FirstOrDefault(x => x.CartDetailId == cartDetailId);
            if (detail != null)
            {
                _context.CartDetails.Remove(detail);
                _context.SaveChanges();
            }
            return RedirectToAction("GioHang");
        }

       
        public IActionResult ThanhToan()
        {
            return View();
        }

    }

}
