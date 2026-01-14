using ECommerce.AdminPanel.Models.Banner;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Banner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

[Authorize]
public class BannerController : Controller
{
    private readonly BaseApiService _apiService;
    public BannerController(BaseApiService apiService)
    {
        _apiService = apiService;
    }

    // BANNER LİSTESİ
    public async Task<IActionResult> Index()
    {
        // API tarafında güncellediğimiz "Banner/List" endpoint'ini çağırıyoruz
        var response = await _apiService.GetAsync<IEnumerable<BannerDto>>("Banner/List");
        return View(response?.Data ?? new List<BannerDto>());
    }

    // YENİ BANNER EKLEME (GET)
    [HttpGet]
    public IActionResult Create() => View(new CreateBannerViewModel { Status = true, Order = 1 });

    // YENİ BANNER EKLEME (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBannerViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Şirket ID'sini token'dan alıp modele ekliyoruz
        var companyIdStr = User.FindFirstValue("companyId");
        Guid? companyId = string.IsNullOrEmpty(companyIdStr) ? null : Guid.Parse(companyIdStr);

        var dto = new BannerCreateDto 
        { 
            Title = model.Title,
            ImageUrl = model.ImageUrl,
            TargetUrl = model.TargetUrl,
            Order = model.Order,
            CompanyId = companyId ?? Guid.Empty, // Adminse genel, şirketse kendi ID'si
            Status = model.Status
        };

        var response = await _apiService.PostAsync<BannerCreateDto, Guid>("Banner/Create", dto);

        if (response != null && response.Success)
        {
            TempData["SuccessMessage"] = "Banner başarıyla yayına alındı.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = response?.Message;
        return View(model);
    }

    // BannerController.cs (MVC)

[HttpGet]
public async Task<IActionResult> Update(Guid id)
{
    // Mevcut banner verisini API'den çekiyoruz
    var response = await _apiService.GetAsync<BannerDto>($"Banner/GetById/{id}");
    
    if (response == null || !response.Success || response.Data == null)
        return NotFound();

    // DTO'yu sayfada kullanacağımız modele dönüştürüyoruz
    var model = new UpdateBannerViewModel
    {
        Id = response.Data.Id,
        Title = response.Data.Title,
        ImageUrl = response.Data.ImageUrl,
        TargetUrl = response.Data.TargetUrl,
        Order = response.Data.Order,
        Status = response.Data.Status,
        CompanyId = response.Data.CompanyId // Şirket ID'sini korumak kritik
    };

    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Update(Guid id, UpdateBannerViewModel model)
{
    if (!ModelState.IsValid) return View(model);

    // API'nin beklediği Update DTO'su
    var dto = new BannerUpdateDto
    {
        Title = model.Title,
        ImageUrl = model.ImageUrl,
        TargetUrl = model.TargetUrl,
        Order = model.Order,
        Status = model.Status,
        CompanyId = model.CompanyId
    };

    var response = await _apiService.PutAsync<BannerUpdateDto, bool>($"Banner/Update/{id}", dto);

    if (response != null && response.Success)
    {
        TempData["SuccessMessage"] = "Banner başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    ViewBag.Error = response?.Message;
    return View(model);
}

    // SİLME İŞLEMİ
    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _apiService.DeleteAsync($"Banner/Delete/{id}");
        if (response != null && response.Success)
            TempData["SuccessMessage"] = "Banner kaldırıldı.";
        
        return RedirectToAction(nameof(Index));
    }
}