using ClosedXML.Excel;
using DATN.IRepository;
using DATN.Middleware;
using DATN.Models;
using DATN.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;
namespace DATN.Controllers
{
    public class UserController(ILogger<UserController> logger, IUsersRepository usersRepository) : Controller
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly ILogger<UserController> _logger = logger;

        [Authorize(Roles = "Admin,Customer")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
                return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([Bind("FullName,Username,Email,Password,Gender,BirthDate,RoleId,PhoneNumber,Address")] User user)
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
                TempData.SetNotification("Cập nhật user thành công!", "success");
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
        [HttpPost]
        public async Task<IActionResult> UpdateUser([Bind("UserId,FullName,Username,Email,Gender,BirthDate,PhoneNumber,RoleId,Address,Status")] User user)
        {
            if(user  == null)
            {
                return BadRequest();
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
                TempData.SetNotification("Cập nhật user thành công!", "warning");
                return View("~/Views/Admin/KhachHang.cshtml", user);
            }
            try
            {
                await _usersRepository.UpdateUser(user);
                TempData.SetNotification("Cập nhật user thành công!", "success");
                return RedirectToAction("KhachHang", "Admin");
            }
            catch (ValidationException ex)
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
                TempData.SetNotification("Đăng ký thành công!", "success");
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

        [HttpGet]
        public IActionResult DangNhap()
        {
            HttpContext.Session.SetInt32("LoginFailCount", 0);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("Email,Password")] User user, string Captcha, string returnUrl)
        {
            int logCount = HttpContext.Session.GetInt32("LoginFailCount") ?? 0;
            // Kiểm tra trạng thái khóa
            if (logCount >= 5)
            {
                if (HttpContext.Session.GetString("LoginLockUntil") == null)
                {
                    HttpContext.Session.SetString("LoginLockUntil", DateTime.UtcNow.AddMinutes(15).ToString());
                }

                var lockTime = DateTime.Parse(HttpContext.Session.GetString("LoginLockUntil")!);
                if (DateTime.UtcNow < lockTime)
                {
                    ViewBag.IsLocked = true;
                    ViewBag.Message = $"Bạn đã bị khóa đăng nhập đến {lockTime.ToLocalTime():HH:mm:ss}.";
                    return View("DangNhap", user);
                }

                // Reset sau khi hết khóa
                HttpContext.Session.SetInt32("LoginFailCount", 0);
                HttpContext.Session.Remove("LoginLockUntil");
                logCount = 0;
            }
            // Kiểm tra thông tin đăng nhập
            bool isValid = _usersRepository.IsValidUser(user.Email ?? "", user.Password ?? "");

            if (!isValid)
            {
                // Tăng đếm trước
                logCount++;
                HttpContext.Session.SetInt32("LoginFailCount", logCount);

                // Bắt CAPTCHA nếu từ lần thứ 3 trở đi
                if (logCount >= 3)
                {
                    ViewBag.ShowCaptcha = true;

                    var storedCaptcha = HttpContext.Session.GetString("CaptchaCode") ?? "";
                    if (string.IsNullOrWhiteSpace(Captcha) || !Captcha.Trim().Equals(storedCaptcha.Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        ViewBag.Message = "Mã xác nhận không đúng.";
                        ModelState.AddModelError("Captcha", ViewBag.Message);
                        return View("DangNhap", user);
                    }
                }

                // Sai tài khoản/mật khẩu
                ViewBag.Message = "Email hoặc mật khẩu không đúng.";
                ModelState.AddModelError("", ViewBag.Message);
                return View("DangNhap", user);
            }
            try {
                // Nếu đã xác thực đúng, lấy thông tin người dùng
                var matchedUser = _usersRepository.GetUserByEmail(user.Email ?? "", user.Password ?? "");
                if (matchedUser == null)
                {
                    ViewBag.Message = "Không thể lấy thông tin người dùng.";
                    ModelState.AddModelError("", ViewBag.Message);
                    return View("DangNhap", user);
                }

                var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, matchedUser.UserId.ToString()),
                        new(ClaimTypes.Email, matchedUser!.Email!),
                        new(ClaimTypes.Name, matchedUser!.FullName!),
                       new(ClaimTypes.Role, matchedUser.Role!.RoleName!.ToLower() ?? "khachhang")
                    };
                Console.WriteLine($"Role: {matchedUser.Role?.RoleName}");

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);
                // Đăng nhập thành công
                HttpContext.Session.SetInt32("UserId", matchedUser!.UserId);
                HttpContext.Session.SetInt32("LoginFailCount", 0); // reset sau khi thành công
                TempData.SetNotification("Đăng nhập thành công!", "success");

                // Nếu ReturnUrl hợp lệ (nội bộ) thì chuyển về, ngược lại về /
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }catch (Exception ex) {
                _logger.LogError(ex, "Lỗi khi đăng nhập.");
                ModelState.AddModelError("", "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
                return View("DangNhap", user);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportAllUsersToExcel(string selectedIds)
        {
            List<User> users;

            if (!string.IsNullOrEmpty(selectedIds))
            {
                var ids = JsonConvert.DeserializeObject<List<int>>(selectedIds);
                users = await _usersRepository.GetUsersByIdsAsync(ids);
            }
            else
            {
                users = (await _usersRepository.GetAllUsers()).ToList();
            }

            return GenerateExcel(users, "users_export.xlsx");
        }
        private FileResult GenerateExcel(List<User> users, string fileName)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Users");

            // Tiêu đề cột
            //worksheet.Row(worksheet.FirstRowUsed()!.RowNumber()).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Value = "Họ tên";
            worksheet.Cell(1, 2).Value = "Email";
            worksheet.Cell(1, 3).Value = "SĐT";
            worksheet.Cell(1, 4).Value = "Số đơn hàng";
            worksheet.Cell(1, 5).Value = "Tổng chi tiêu";
            worksheet.Cell(1, 6).Value = "Trạng thái";
            worksheet.Cell(1, 7).Value = "Ngày tham gia";

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                var row = i + 2;

                var orderCount = user.Orders?.Count ?? 0;
                var totalAmount = user.Orders?.Sum(o => o.TotalAmount) ?? 0;
                var status = orderCount >= 10 ? "VIP" :
                             orderCount >= 5 ? "Thường xuyên" : "Mới";

                worksheet.Cell(row, 1).Value = user.FullName;
                worksheet.Cell(row, 2).Value = user.Email;
                worksheet.Cell(row, 3).Value = user.PhoneNumber;
                worksheet.Cell(row, 4).Value = orderCount;
                worksheet.Cell(row, 5).Value = totalAmount;
                worksheet.Cell(row, 6).Value = status;
                worksheet.Cell(row, 7).Value = user.CreatedDate.ToString("dd/MM/yyyy");
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream); // hoặc package.SaveAs(stream)
            stream.Position = 0;     // rất quan trọng: reset về đầu stream

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }


        [HttpPost]
        public IActionResult Logout()
        {          
            HttpContext.SignOutAsync("MyCookieAuth");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "User");
        }
        public IActionResult KhongDuQuyen()
        {
            return View(); // Thông báo bạn không có quyền
        }
        public IActionResult DangKy()
        {
            return View();
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
        public IActionResult QuenMatKhau()
        {
            return View();
        }   
        //public IActionResult ChiTiet()
        //{
        //    return View();
        //}
    }
}
