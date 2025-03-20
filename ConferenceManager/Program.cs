using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using ConferenceManager.Data; // ✅ Добави namespace за ApplicationDbContext

var builder = WebApplication.CreateBuilder(args);

// Конфигуриране на базата данни
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавяне на Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Конфигуриране на Authentication и Authorization
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Път към страницата за вход
});

// Добавяне на MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // 🔥 Грешките ще се насочват към Error контролера
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 🔥 Много важно! Активира аутентикацията
app.UseAuthorization();  // 🔥 Активира авторизацията

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
