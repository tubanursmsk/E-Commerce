using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Models.Products; // Kendi namespace'ine göre güncelle
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

[Authorize] // Sadece giriş yapanlar erişebilir
public class ProductController : Controller
{
    private readonly BaseApiService _apiService;

    public ProductController(BaseApiService apiService)
    {
        _apiService = apiService;
    }

    // ÜRÜN LİSTESİ
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        // Not: API tarafındaki GetAll metodun sayfalama desteklemiyorsa düz liste çekebiliriz
        // Şablonun beklediği 'ProductListViewModel' yapısını dolduruyoruz
        var response = await _apiService.GetAsync<IEnumerable<ProductDto>>("Product/List");

        var model = new ProductListViewModel
        {
            Products = new PagedResult<ProductDto> // Şablondaki yapıya uygun sarmalıyoruz
            {
                Items = response?.Data ?? new List<ProductDto>(),
                TotalCount = response?.Data?.Count() ?? 0,
                PageNumber = page,
                PageSize = pageSize
            }
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // 1. Kategorileri Çek API'den veri gelmezse boş liste gönder ki View patlamasın
        var categoryResponse = await _apiService.GetAsync<IEnumerable<CategoryDto>>("Category/List");
        ViewBag.AllCategories = categoryResponse?.Data?.ToList() ?? new List<CategoryDto>();

        // 2. Markaları Çek (Eksik olan kısım burasıydı)
        var brandResponse = await _apiService.GetAsync<IEnumerable<BrandDto>>("Brand/List"); // API endpoint'inin doğruluğundan emin ol
        ViewBag.AllBrands = brandResponse?.Data?.ToList() ?? new List<BrandDto>();

        if ((categoryResponse != null && !categoryResponse.Success) || (brandResponse != null && !brandResponse.Success))
        {
            TempData["ErrorMessage"] = "Veriler yüklenirken bir sorun oluştu.";
        }

        // CompanyId'yi Claims veya Session'dan alıp modele ekliyoruz
        var companyIdStr = User.FindFirst("CompanyId")?.Value ?? HttpContext.Session.GetString("CompanyId");
        Guid.TryParse(companyIdStr, out var companyId);

        var model = new CreateProductViewModel { CompanyId = companyId };
        return View(model);
    }

    // YENİ ÜRÜN OLUŞTURMA (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var categoryResponse = await _apiService.GetAsync<IEnumerable<CategoryDto>>("Category/List");
            ViewBag.AllCategories = categoryResponse?.Data?.ToList();
            var brandResponse = await _apiService.GetAsync<IEnumerable<BrandDto>>("Brand/List");
            ViewBag.AllBrands = brandResponse?.Data?.ToList();

            return View(model);
        }
        // MVC Modelini API'nin beklediği DTO'ya map ediyoruz
        var productDto = new ProductCreateDto
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            Stock = model.Stock,
            CategoryId = model.CategoryId,
            BrandId = model.BrandId,
            CompanyId = model.CompanyId
        };

        var response = await _apiService.PostAsync<ProductCreateDto, Guid>("Product/Create", productDto);
        if (response != null && response.Success)
        {
            TempData["SuccessMessage"] = "Ürün başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }
        // Hata durumunda kategorileri tekrar yükle
        var cats = await _apiService.GetAsync<IEnumerable<CategoryDto>>("Category/List");
        ViewBag.AllCategories = cats?.Data?.ToList() ?? new List<CategoryDto>();
        var brands = await _apiService.GetAsync<IEnumerable<BrandDto>>("Brand/List");
        ViewBag.AllBrands = brands?.Data?.ToList() ?? new List<BrandDto>();
        ViewBag.Error = response?.Message ?? "API tarafında bir hata oluştu.";
        return View(model);
    }

    // ÜRÜN SİLME
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        // API tarafında Delete metodu genellikle DELETE HTTP fiiliyle çalışır
        // BaseApiService'e DeleteAsync eklemediysek PostAsync ile 'Product/Delete/id' çağrılabilir
        // Şimdilik simüle ediyoruz:
        var response = await _apiService.PostAsync<object, bool>($"Product/Delete/{id}", new { });

        if (response != null && response.Success)
            TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
        else
            TempData["ErrorMessage"] = response?.Message ?? "Ürün silinemedi.";

        return RedirectToAction(nameof(Index));
    }
}