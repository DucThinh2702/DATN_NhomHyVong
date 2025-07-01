using DATN.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using DATN.IRepository;
using System.Security.Claims;
namespace DATN.Controllers
{
    public class UserController(ILogger<UserController> logger, IUsersRepository usersRepository) : Controller
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly ILogger<UserController> _logger = logger;

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _usersRepository.GetAllUsers();
                ViewData["User"] = users;
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User cannot be null");
                }
                var createdUser = await _usersRepository.CreateUser(user);
                return RedirectToAction("Index","Admin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _usersRepository.GetUserById(id);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by ID");
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> DetailUserById(int id)
        {
            try
            {
                var user = await _usersRepository.GetUserById(id);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by ID");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (_usersRepository.EmailExists(model.Email??"Sai email"))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Username = model.Username,
                Email = model.Email,
                Password = model.Password, // Gợi ý: nên mã hóa
                Gender = model.Gender,
                BirthDate = DateOnly.FromDateTime(model.BirthDate),
                PhoneNumber = model.PhoneNumber,
                Address = model.Adddress,
                RoleId = 1, // mặc định User role
                CreatedDate = DateTime.Now,
                Status = true
            };

            await _usersRepository.RegisterAsync(user);

            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User cannot be null");
                }
                await _usersRepository.UpdateUser(user);
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _usersRepository.DeleteUser(id);
                ViewBag.Message = "User deleted successfully";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return View("Error");
            }
        }
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string pass)
        {
            try
            {
                var user = _usersRepository.GetUserByEmail(email, pass);
                if (user != null)
                {
                    // Lưu UserId vào Session
                    HttpContext.Session.SetInt32("UserId", user.UserId); // Assumes UserId is int

                    // Tạo Claims (tuỳ chọn nếu dùng Claims-based Authentication)
                    var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.FullName ?? user.Username ?? "Unknown"),
                new(ClaimTypes.Email, user.Email ?? "Unknown"),
                new(ClaimTypes.Role, user.Role?.RoleName ?? "User")
            };

                    // Có thể thêm xác thực bằng cookie ở đây nếu cần

                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid email or password.";
                    return View("DangNhap","User");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return View("Error");
            }
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xoá tất cả session (bao gồm UserId)
            return RedirectToAction("Index", "User");
        }

        //public IActionResult GioHang()
        //{
        //    return View();
        //}
        //public IActionResult ThanhToan()
        //{
        //    return View();
        //}
        //public IActionResult LienHe()
        //{
        //    return View();
        //}
        //public IActionResult QuenMatKhau()
        //{
        //    return View();
        //}
        
        //public IActionResult ChiTiet()
        //{
        //    return View();
        //}
    }
}
