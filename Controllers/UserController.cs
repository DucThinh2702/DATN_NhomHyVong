using DATN.IRepository;
using DATN.Models;
using DATN.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public async Task<IActionResult> CreateUser([Bind("FullName,Username,Email,Password,Gender,BirthDate,PhoneNumber,Address")] User user)
        {
            // Kiểm tra thủ công và gom lỗi
            var errorMessages = new List<string>();

            if (!string.IsNullOrEmpty(user.Email) && await _usersRepository.IsEmailExistsAsync(user.Email))
                errorMessages.Add("Email đã tồn tại");

            if (!string.IsNullOrEmpty(user.Username) && await _usersRepository.IsUsernameExistsAsync(user.Username))
                errorMessages.Add("Username đã tồn tại");

            if (!string.IsNullOrEmpty(user.PhoneNumber) && await _usersRepository.IsPhoneNumberExistsAsync(user.PhoneNumber))
                errorMessages.Add("Số điện thoại đã tồn tại");

            if (errorMessages.Count > 0)
            {
                ViewBag.FormSubmitted = true;
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessages = errorMessages;
                return View("~/Views/Admin/KhachHang.cshtml", user);
            }

            try
            {
                await _usersRepository.CreateUser(user);
                return RedirectToAction("KhachHang", "Admin");
            }
            catch (InvalidOperationException ex)
            {
                ViewBag.FormSubmitted = true;
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessages = new List<string> { ex.Message };
                return View("~/Views/Admin/KhachHang.cshtml", user);
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
            // Kiểm tra thủ công và gom lỗi
            var errorMessages = new List<string>();

            if (!string.IsNullOrEmpty(model.Email) && await _usersRepository.IsEmailExistsAsync(model.Email))
                errorMessages.Add("Email đã tồn tại");

            if (!string.IsNullOrEmpty(model.Username) && await _usersRepository.IsUsernameExistsAsync(model.Username))
                errorMessages.Add("Username đã tồn tại");

            if (!string.IsNullOrEmpty(model.PhoneNumber) && await _usersRepository.IsPhoneNumberExistsAsync(model.PhoneNumber))
                errorMessages.Add("Số điện thoại đã tồn tại");

            if (errorMessages.Count > 0)
            {
                ViewBag.FormSubmitted = true;
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessages = errorMessages;
                return View("~/Views/User/DangKy.cshtml", model);
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
            try
            {
                await _usersRepository.RegisterAsync(user);
                return RedirectToAction("DangKy", "User");
            }
            catch (InvalidOperationException ex)
            {

                ViewBag.FormSubmitted = true;
                ViewBag.HasErrors = true;
                 ViewBag.ErrorMessages = new List<string> { ex.Message };
                return View("~/Views/User/DangKy.cshtml", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([Bind("UserId,FullName,Username,Email,Gender,BirthDate,PhoneNumber,Address,RoleId,Status")] User user)
        {
            if (user == null)
            {
                return BadRequest("User cannot be null");
            }
            var errorMessagesUpdate = new List<string>();
            //Isemailexistasync có id
            if (!string.IsNullOrEmpty(user.Email) && await _usersRepository.IsEmailExistsAsync(user.Email, user.UserId))
                errorMessagesUpdate.Add("Email đã tồn tại");

            if (!string.IsNullOrEmpty(user.Username) && await _usersRepository.IsUsernameExistsAsync(user.Username, user.UserId))
                errorMessagesUpdate.Add("Username đã tồn tại");

            if (!string.IsNullOrEmpty(user.PhoneNumber) && await _usersRepository.IsPhoneNumberExistsAsync(user.PhoneNumber, user.UserId))
                errorMessagesUpdate.Add("Số điện thoại đã tồn tại");


            if (errorMessagesUpdate.Count > 0)
            {
                ViewBag.FormSubmitted = true;
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessages = errorMessagesUpdate;
                return View("~/Views/Admin/KhachHang.cshtml", user);
            }
            try
            {
                await _usersRepository.UpdateUser(user);
                return RedirectToAction("KhachHang", "Admin");
            }
            catch (ValidationException ex)
            {
                ViewBag.FormSubmitted = true;
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessages = new List<string> { ex.Message};
                return View("~/Views/Admin/KhachHang.cshtml", user);
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
        public IActionResult Login([Bind("Email,Password")] User user)
        {
            try
            {
                var matchedUser = _usersRepository.GetUserByEmail(user.Email ?? "", user.Password ?? "");

                HttpContext.Session.SetInt32("UserId", matchedUser.UserId);
                return RedirectToAction("Index", "Home");
            }
            catch (KeyNotFoundException)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(user);
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
