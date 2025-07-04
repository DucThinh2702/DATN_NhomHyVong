using DATN.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ✅ Đăng ký DbContext cho Entity Framework Core
builder.Services.AddDbContext<DatnContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Đăng ký MVC
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatnContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ✅ Cấu hình middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ✅ Cấu hình route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=SanPham}/{id?}");

app.Run();
