using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Models.Brands;
using ECommerce.AdminPanel.Models.Products;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

[Authorize]
public class BrandController : Controller
{
    private readonly BaseApiService _apiService;
    public BrandController(BaseApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        // Manager ise sadece kendi şirketinin markaları gelir (API'de filtrelediysen)
        var response = await _apiService.GetAsync<IEnumerable<BrandDto>>("Brand/List");
        var model = new BrandListViewModel { Brands = response?.Data ?? new List<BrandDto>() };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // Üst kategori seçebilmesi için mevcut kategorileri çekiyoruz
        var response = await _apiService.GetAsync<IEnumerable<BrandDto>>("Brand/List");
        ViewBag.ParentBrand = response?.Data?.ToList() ?? new List<BrandDto>();

        return View(new BrandViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BrandViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Oturumdaki CompanyId'yi alıyoruz
        var companyIdStr = User.FindFirstValue("companyId") ?? HttpContext.Session.GetString("companyId");
        model.CompanyId = Guid.Parse(companyIdStr ?? Guid.Empty.ToString());

        // API'ye gönderiyoruz
        var response = await _apiService.PostAsync<BrandViewModel, Guid>("Brand/Create", model);

        if (response != null && response.Success)
        {
            TempData["SuccessMessage"] = "Marka başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = response?.Message ?? "Marka eklenirken bir hata oluştu.";
        return View(model);
    }

    //Marka DÜZENLEME (GET)
    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        // API'den mevcut markayı getir
        var response = await _apiService.GetAsync<BrandDto>($"Brand/GetById/{id}");

        if (response == null || !response.Success)
        {
            TempData["ErrorMessage"] = "Marka bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var brand = response.Data;
        var model = new UpdateBrandViewModel
        {
            Id = brand.Id,
            Name = brand.Name,
            LogoUrl = brand.LogoUrl,
            CompanyId = brand.CompanyId
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateBrandViewModel model)
    {
        // CompanyId'yi sadece Session'dan al
        var companyIdStr = HttpContext.Session.GetString("companyId");

        if (string.IsNullOrWhiteSpace(companyIdStr) || !Guid.TryParse(companyIdStr, out Guid companyId))
        {
            TempData["ErrorMessage"] = "Firma bilgisi bulunamadı. Lütfen tekrar giriş yapın.";
            return RedirectToAction("Login", "Auth");
        }

        model.CompanyId = companyId;

        if (!ModelState.IsValid)
            return View(model);

        var updateDto = new BrandUpdateDto
        {
            Name = model.Name,
            LogoUrl = model.LogoUrl,
            CompanyId = model.CompanyId //modelden aldık (BrandUpdateDto)
        };

        var response = await _apiService.PutAsync<BrandUpdateDto, bool>($"Brand/Update/{model.Id}", updateDto);

        if (response is { Success: true })
        {
            TempData["SuccessMessage"] = "Marka başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = response?.Message ?? "Güncelleme sırasında hata oluştu.";
        return View(model);
    }

    // ÜRÜN SİLME
    [HttpPost] // View'dan gelen form isteği POST'tur
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        // API'ye DELETE isteği gönderiyoruz
        // BaseApiService içindeki DeleteAsync metodunu çağırmalıyız
        var response = await _apiService.DeleteAsync($"Brand/Delete/{id}");

        if (response != null && response.Success)
        {
            TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
        }
        else
        {
            TempData["ErrorMessage"] = response?.Message ?? "Ürün silinemedi.";
        }

        return RedirectToAction(nameof(Index));
    }
}