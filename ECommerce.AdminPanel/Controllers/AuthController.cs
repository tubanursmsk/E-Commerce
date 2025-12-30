using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

public class AuthController : Controller
{
    private readonly BaseApiService _apiService;

    public AuthController(BaseApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // API'ye gidiyoruz (Senin LoginDto yapına uygun gönderiyoruz)
        var loginRequest = new { Email = model.EmailOrUserName, Password = model.Password };
        var response = await _apiService.PostAsync<object, string>("Auth/Login", loginRequest);

        if (response != null && response.Success && !string.IsNullOrEmpty(response.Data))
        {
            var token = response.Data;
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Claimleri alıyoruz
            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "unique_name")?.Value ?? model.EmailOrUserName;
            var companyId = jwtToken.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // 1. Session Set Et (Layout'taki @Context.Session.GetString kodları için)
            HttpContext.Session.SetString("UserName", userName);
            if (companyId != null) HttpContext.Session.SetString("CompanyId", companyId);

            // 2. Cookie Auth (Yetkilendirme için)
            var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, role ?? "Customer")
        };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // 3. API Token'ı Cookie'ye yaz (BaseApiService için)
            Response.Cookies.Append("JwtToken", token, new CookieOptions { HttpOnly = true });

            TempData["SuccessMessage"] = "Hoş geldiniz, giriş başarılı!";
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = response?.Message ?? "E-posta veya şifre hatalı.";
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("JwtToken");
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }


    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterCompanyViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // API'nin beklediği Register DTO formatına dönüştürüyoruz
        // Not: API tarafındaki Register metodunun hem user hem company bilgilerini aldığını varsayıyoruz
        var result = await _apiService.PostAsync<RegisterCompanyViewModel, string>("Auth/Register", model);

        if (result != null && result.Success)
        {
            TempData["SuccessMessage"] = "Şirket kaydınız başarıyla oluşturuldu. Giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        ViewBag.Error = result?.Message ?? "Kayıt işlemi sırasında bir hata oluştu.";
        return View(model);
    }

}
/* 

Özetle Mantık Şöyle İşler:
Giriş: Kullanıcı MVC'ye yazar -> MVC API'ye sorar.

Yanıt: API bir JWT verir.

Parselleme: MVC bu JWT'yi açar; "Ha bu adam Admin'miş, şirketi de X'miş" der ve bunu kendi Cookie'sine yazar.

Kullanım: Sen yarın bir sayfaya [Authorize(Roles="Admin")] yazdığında, MVC API'ye gitmez, kendi Cookie'sine bakar.

Servis: Bir veri istediğinde BaseApiService devreye girer, "Dur, şu JWT'yi de yanıma alayım da API beni tanısın" der.

*/