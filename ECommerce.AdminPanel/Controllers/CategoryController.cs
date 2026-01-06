using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Models.Categories;
using ECommerce.AdminPanel.Models.Products;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.DTOs.Product;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

[Authorize]
public class CategoryController : Controller
{
    private readonly BaseApiService _apiService;

    public CategoryController(BaseApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        // Manager ise sadece kendi şirketinin kategorileri gelir (API'de filtrelediysen)
        var response = await _apiService.GetAsync<IEnumerable<CategoryDto>>("Category/List");
        var model = new CategoryListViewModel { Categories = response?.Data ?? new List<CategoryDto>() };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // Üst kategori seçebilmesi için mevcut kategorileri çekiyoruz
        var response = await _apiService.GetAsync<IEnumerable<CategoryDto>>("Category/List");
        ViewBag.ParentCategories = response?.Data?.ToList() ?? new List<CategoryDto>();

        return View(new CategoryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Oturumdaki CompanyId'yi alıyoruz
        var companyIdStr = User.FindFirstValue("companyId") ?? HttpContext.Session.GetString("companyId");
        model.CompanyId = Guid.Parse(companyIdStr ?? Guid.Empty.ToString());

        // API'ye gönderiyoruz
        var response = await _apiService.PostAsync<CategoryViewModel, Guid>("Category/Create", model);

        if (response != null && response.Success)
        {
            TempData["SuccessMessage"] = "Kategori başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = response?.Message ?? "Kategori eklenirken bir hata oluştu.";
        return View(model);
    }

    // KATEGORİ DÜZENLEME (GET)
    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        // API'den mevcut kategoriyi getir
        var response = await _apiService.GetAsync<CategoryDto>($"Category/GetById/{id}");

        if (response == null || !response.Success)
        {
            TempData["ErrorMessage"] = "Kategori bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var category = response.Data;
        var model = new UpdateCategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CompanyId = category.CompanyId
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateCategoryViewModel model)
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

        var updateDto = new CategoryUpdateDto
        {
            Name = model.Name,
            Description = model.Description,
            CompanyId = model.CompanyId
        };

        var response = await _apiService.PostAsync<CategoryUpdateDto, bool>($"Category/Update/{model.Id}", updateDto);

        if (response is { Success: true })
        {
            TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
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
        var response = await _apiService.DeleteAsync($"Category/Delete/{id}");

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