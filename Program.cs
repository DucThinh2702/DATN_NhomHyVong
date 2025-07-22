using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

builder.Services.AddDbContext<DATN.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));
builder.Services.AddScoped<DATN.IRepository.IUsersRepository, DATN.Service.UsersService>();
builder.Services.AddAuthentication("MyCookieAuth")
        .AddCookie("MyCookieAuth", options =>
        {
            options.LoginPath = "/User/DangNhap";
            options.LogoutPath = "/User/DangXuat";
            options.AccessDeniedPath = "/User/KhongDuQuyen"; // ← thêm dòng này
                                                             // Thời gian sống của cookie
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);   // 60 phút
            options.SlidingExpiration = true;
            // (Tuỳ chọn) Chỉ gửi cookie qua HTTPS
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout to 30 minutes
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication(); // <- Bắt
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");


app.Run();
