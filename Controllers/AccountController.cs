using DATN.Data;
using DATN.Models;
using DATN.Models.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DATN.Extensions;

namespace DATN.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatnContext _context;

        public AccountController(DatnContext context)
        {
            _context = context;
        }

        // Trang đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // Xử lý đăng ký
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu người dùng đã tồn tại
                var existingUser = _context.Users.SingleOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                    return View(model);
                }

                // Tạo Salt ngẫu nhiên
                var salt = new byte[128 / 8]; // Salt 16 bytes
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt); // Tạo salt ngẫu nhiên
                }

                // Mã hóa mật khẩu với salt
                var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: model.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256, // Sử dụng HMACSHA256 thay cho HMACSHA1
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                // Lưu thông tin người dùng vào CSDL
                var user = new User
                {
                    FullName = model.FullName,
                    Username = model.Username,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Address = model.Address,
                    BirthDate = model.BirthDate.HasValue ? DateOnly.FromDateTime(model.BirthDate.Value) : (DateOnly?)null,
                    CreatedDate = DateTime.Now,
                    RoleId = 2,  // Mặc định là User
                    Status = false,  // Chưa xác thực
                    Password = hashedPassword, // Lưu mật khẩu đã mã hóa
                    Salt = Convert.ToBase64String(salt) // Lưu Salt vào cơ sở dữ liệu
                };

                // Lưu vào CSDL
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Lưu OTP vào session để so sánh khi xác thực
                string otp = GenerateOtp();
                HttpContext.Session.SetString("otp", otp);

                // Gửi OTP qua email
                if (!string.IsNullOrEmpty(user.Email))
                {
                    await SendOtpAsync(user.Email, otp);  // Gửi qua email
                }

                // Lưu thông tin người dùng tạm thời vào session
                HttpContext.Session.SetObject("user", user);

                // Redirect đến trang xác thực OTP
                return RedirectToAction("VerifyOtp", new { userId = user.UserId });
            }

            return View(model);
        }





        // Trang xác thực OTP
        [HttpGet]
        public IActionResult VerifyOtp(int userId)
        {
            return View(new VerifyOtpViewModel { UserId = userId });
        }

        // Xử lý xác thực OTP
        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            var otp = HttpContext.Session.GetString("otp");

            if (otp != null && otp == model.Otp)
            {
                var user = HttpContext.Session.GetObject<User>("user");
                if (user != null)
                {
                    // Cập nhật trạng thái tài khoản thành đã xác thực
                    user.Status = true;

                    // Chỉ cập nhật trạng thái, không thay đổi mật khẩu mã hóa
                    _context.Users.Update(user);  // Sử dụng Update thay vì Add
                    await _context.SaveChangesAsync();

                    // Xóa thông tin tạm thời trong session
                    HttpContext.Session.Remove("user");
                    HttpContext.Session.Remove("otp");

                    // Redirect đến trang đăng nhập
                    return RedirectToAction("Login");
                }
            }

            ModelState.AddModelError("", "Mã OTP không chính xác.");
            return View(model);
        }





        // Trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // Xử lý đăng nhập
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra đăng nhập bằng Email
                var user = _context.Users.SingleOrDefault(u => u.Email == model.Email);

                if (user != null)
                {
                    // Kiểm tra trạng thái xác thực
                    if (user.Status == false) // Tài khoản chưa xác thực
                    {
                        ModelState.AddModelError("", "Tài khoản chưa được xác thực. Vui lòng kiểm tra email để xác thực tài khoản.");
                        return View(model);
                    }

                    // Kiểm tra mật khẩu
                    bool isPasswordValid = VerifyPassword(model.Password, user.Password, user.Salt);
                    if (isPasswordValid)
                    {
                        // Đăng nhập thành công
                        if (user.RoleId == 1) // Admin
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (user.RoleId == 2) // User
                        {
                            return RedirectToAction("Index", "User");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu không chính xác.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email không tồn tại.");
                }
            }

            return View(model);
        }




        // Tạo mã OTP ngẫu nhiên
        private string GenerateOtp()
        {
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            return otp;
        }

        // Gửi OTP qua email
        private async Task SendOtpAsync(string email, string otp)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("NoReply", "no-reply@example.com"));
            emailMessage.To.Add(new MailboxAddress("", email));  // "" là tên hiển thị, có thể để rỗng
            emailMessage.Subject = "Mã OTP xác thực";
            emailMessage.Body = new TextPart("plain")
            {
                Text = $"Mã OTP của bạn là: {otp}"
            };

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls); // Gmail SMTP server
                await smtp.AuthenticateAsync("aklaakthoi9@gmail.com", "drwt fawk ybao kpdz"); // Dùng mật khẩu ứng dụng nếu xác minh 2 bước bật
                await smtp.SendAsync(emailMessage);
                await smtp.DisconnectAsync(true);
            }
        }

        // Kiểm tra mật khẩu
        private bool VerifyPassword(string enteredPassword, string storedPassword, string storedSaltBase64)
        {
            // Chuyển đổi Salt từ Base64 về mảng byte
            var storedSalt = Convert.FromBase64String(storedSaltBase64);

            // Mã hóa mật khẩu nhập vào với Salt từ cơ sở dữ liệu
            var hashedEnteredPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,            // Mật khẩu người dùng nhập vào
                salt: storedSalt,                     // Salt đã lưu trong cơ sở dữ liệu
                prf: KeyDerivationPrf.HMACSHA256,     // Sử dụng HMACSHA256
                iterationCount: 10000,                // Số vòng lặp
                numBytesRequested: 256 / 8));         // Số byte yêu cầu

            // So sánh mật khẩu đã mã hóa
            Console.WriteLine($"Stored Password (from DB): {storedPassword}");
            Console.WriteLine($"Hashed Entered Password: {hashedEnteredPassword}");

            return hashedEnteredPassword == storedPassword;
        }


    }
}
