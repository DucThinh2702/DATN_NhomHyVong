using DATN.Data;
using DATN.IRepository;
using DATN.Models;
using DATN.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DATN.Controllers
{
    public class AdminController(ILogger<AdminController> logger, IUsersRepository context) : Controller
    {
        private readonly IUsersRepository _context = context;
        private readonly ILogger<AdminController> _logger = logger;

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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> KhachHang(string? searchName, string? filterBy, int page = 1, int pageSize = 6)
        {
            var listUsers = await _context.GetAllUsers();
            // Tùy tình huống, có thể delay ở đây
            
            // 1. Tìm kiếm
            if (!string.IsNullOrEmpty(searchName))
            {
                listUsers = listUsers
                    .Where(u => !string.IsNullOrEmpty(u.FullName) &&
                                u.FullName.Contains(searchName, StringComparison.OrdinalIgnoreCase))
                    .ToList()!;
            }
           
            // 2. Lọc theo điều kiện
            if (!string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy.ToLower())
                {
                    case "money":
                        listUsers = listUsers
                            .OrderByDescending(u => u.Orders?.Sum(o => o.TotalAmount) ?? 0)
                            .ToList()!;
                        break;
                    case "date":
                        listUsers = listUsers
                            .OrderByDescending(u => u.CreatedDate)
                            .ToList()!;
                        break;
                }
            }

            // 3. Tổng số bản ghi sau tìm & lọc
            int totalUsers = listUsers.Count(); // dùng LINQ
            int totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            // 4. Phân trang
            var usersPaged = listUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 5. Truyền dữ liệu sang View
            ViewBag.Users = usersPaged;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
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
        [HttpGet]
        public async Task<JsonResult> IsEmailExists(string email, int? excludeUserId)
        {
            bool exists = excludeUserId.HasValue
                ? await _context.IsEmailExistsAsync(email, excludeUserId.Value)
                : await _context.IsEmailExistsAsync(email);

            return Json(new { exists });
        }

        [HttpGet]
        public async Task<JsonResult> IsUsernameExists(string username, int? excludeUserId)
        {
            bool exists = excludeUserId.HasValue
                ? await _context.IsUsernameExistsAsync(username, excludeUserId.Value)
                : await _context.IsUsernameExistsAsync(username);

            return Json(new { exists });
        }

        [HttpGet]
        public async Task<JsonResult> IsPhoneExists(string phone, int? excludeUserId)
        {
            bool exists = excludeUserId.HasValue
                ? await _context.IsPhoneNumberExistsAsync(phone, excludeUserId.Value)
                : await _context.IsPhoneNumberExistsAsync(phone);

            return Json(new { exists });
        }

    }
}
