using DATN.Data;
using DATN.Repositories;
using DATN.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ✅ Đăng ký DbContext cho Entity Framework Core
builder.Services.AddDbContext<DatnContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DapperHelper>();
builder.Services.AddSingleton<OrderRepository>();
builder.Services.AddSingleton<OrderDetailRepository>();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
// Add services to the container.

// Cấu hình DbContext cho ứng dụng
builder.Services.AddDbContext<DatnContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()); // Kích hoạt Sensitive Data Logging

// Cấu hình session nếu bạn dùng OTP hoặc giữ thông tin tạm
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // Thời gian hết hạn của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình dịch vụ MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddViewOptions(options => {
    options.HtmlHelperOptions.ClientValidationEnabled = true;
});

// Cấu hình session để lưu trữ OTP (trong trường hợp cần gửi OTP qua email)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn của session OTP
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Thêm dịch vụ để hỗ trợ các tính năng ClientValidation
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Cấu hình HSTS cho môi trường production
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Đảm bảo sử dụng session để lưu OTP
app.UseSession();

// Đảm bảo sử dụng session để lưu OTP
app.UseSession();

app.UseRouting();

// Thêm xác thực và phân quyền
app.UseAuthentication();  // Thêm middleware cho xác thực
app.UseAuthorization();   // Thêm middleware cho phân quyền

// Cấu hình các route cho Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Orders}/{action=Index}/{id?}");

app.Run();
