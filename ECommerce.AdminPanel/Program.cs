using ECommerce.AdminPanel.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC Controller ve View desteği
builder.Services.AddControllersWithViews();

// 2. HttpContextAccessor (Cookie okumak için lazım)
builder.Services.AddHttpContextAccessor();

// 1. ADIM: Session servisini kaydet
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 dakika inaktif kalırsa silinir
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. HttpClient ve BaseApiService Kaydı
builder.Services.AddHttpClient<BaseApiService>();

// 4. MVC için Cookie Auth Yapılandırması
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Giriş yapılmamışsa yönlendirilecek sayfa
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "ECommerceAdminCookie";
    });

// (Opsiyonel ama temiz)
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Development'ta HTTP kullanıldığı için HTTPS redirect kapalı
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// Auth sıralaması önemli!
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

app.Run();