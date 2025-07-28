

////using DATN.Data; // Nếu AppDbContext ở Data
////using Microsoft.EntityFrameworkCore;


////var builder = WebApplication.CreateBuilder(args);

////var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

////// ✅ Đăng ký DbContext cho Entity Framework Core
////builder.Services.AddDbContext<AppDbContext>(options =>
////    options.UseSqlServer(connectionString));

////// ✅ Đăng ký MVC
////builder.Services.AddControllersWithViews();

////var app = builder.Build();

////// ✅ Cấu hình middleware pipeline
////if (!app.Environment.IsDevelopment())
////{
////    app.UseExceptionHandler("/Home/Error");
////    app.UseHsts();
////}

////app.UseHttpsRedirection();
////app.UseStaticFiles();

////app.UseRouting();

////app.UseAuthorization();

////// ✅ Cấu hình route mặc định
////app.MapControllerRoute(
////    name: "default",
////    pattern: "{controller=Products1}/{action=Index}/{id?}");

////app.Run();


//using DATN.Data;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//// Đăng ký DatnDbContext (đúng tên class bạn có)
//builder.Services.AddDbContext<DatnDbContext>(options =>
//    options.UseSqlServer(connectionString));

//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseAuthorization();


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Products1}/{action=Index}/{id?}");

//app.Run();


using DATN.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Kết nối database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DatnDbContext>(options =>
    options.UseSqlServer(connectionString));

// Thêm Session (quan trọng)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Bật session trước Authorization
app.UseSession();

app.UseAuthorization();

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products1}/{action=Index}/{id?}");

app.Run();
