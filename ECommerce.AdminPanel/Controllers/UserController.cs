using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Models.User;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

[Authorize(Roles = "Admin,CompanyManager")]
public class UserController : Controller
{
    private readonly BaseApiService _apiService;
    public UserController(BaseApiService apiService) => _apiService = apiService;

    public async Task<IActionResult> Index()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        ApiResponse<IEnumerable<UserDto>> response;

        if (role == "Admin")
            response = await _apiService.GetAsync<IEnumerable<UserDto>>("User/All");
        else
        {
            var companyId = User.FindFirstValue("companyId");
            response = await _apiService.GetAsync<IEnumerable<UserDto>>($"User/CompanyStaff/{companyId}");
        }

        return View(response?.Data ?? new List<UserDto>());
    }

    [HttpGet]
    public IActionResult Create() => View(new UserCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Giriş yapan yöneticinin şirket ID'sini alıyoruz
        var companyIdStr = User.FindFirstValue("companyId");
        Guid? companyId = string.IsNullOrEmpty(companyIdStr) ? null : Guid.Parse(companyIdStr);

        // API'ye gidecek Register DTO'su (veya UserCreateDto)
        var dto = new UserCreateDto
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Password = model.Password,
            Role = model.Role,
            CompanyId = companyId // Admin ise null gidebilir, Manager ise kendi şirketi
        };

        // AuthController içindeki Register metodunu veya UserService'deki özel Create metodunu çağırıyoruz
        var response = await _apiService.PostAsync<object, Guid>("Auth/UserRegister", dto);

        if (response != null && response.Success)
        {
            TempData["SuccessMessage"] = "Yeni personel başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = response?.Message;
        return View(model);
    }



[HttpGet]
public async Task<IActionResult> Profile()
{
    // Giriş yapan kullanıcının ID'sini alıyoruz
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

    var response = await _apiService.GetAsync<UserDto>($"User/{userId}");
    
    if (response == null || !response.Success) return NotFound();

    var model = new UserProfileViewModel
    {
        Id = response.Data.Id,
        FirstName = response.Data.FirstName,
        LastName = response.Data.LastName,
        Email = response.Data.Email,
        Role = response.Data.Role
    };

    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Profile(UserProfileViewModel model)
{
    if (!ModelState.IsValid) return View(model);

    var dto = new UserUpdateDto
    {
        Id = model.Id,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        Role = model.Role, // Rolü değiştirmemesine rağmen DTO'da gerekli olabilir
        Status = true
    };

    var response = await _apiService.PutAsync<UserUpdateDto, bool>($"User/UpdateProfile/{model.Id}", dto);

    if (response != null && response.Success)
    {
        TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
        return RedirectToAction(nameof(Profile));
    }

    ViewBag.Error = response?.Message;
    return View(model);
}
}