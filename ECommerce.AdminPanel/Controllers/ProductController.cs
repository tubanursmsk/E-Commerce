using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Models.Products; // Kendi namespace'ine göre güncelle
using ECommerce.AdminPanel.Services;
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
    try
    {
        // Kategori listesini çek
        var categoryResponse = await _apiService.GetAsync<IEnumerable<CategoryDto>>("Category/List");
        
        if (categoryResponse == null || !categoryResponse.Success)
        {
            // Hata mesajını göster
            TempData["ErrorMessage"] = categoryResponse?.Message ?? "Kategoriler yüklenemedi.";
            ViewBag.AllCategories = new List<CategoryDto>();
        }
        else
        {
            ViewBag.AllCategories = categoryResponse.Data?.ToList() ?? new List<CategoryDto>();
        }
        
        var model = new CreateProductViewModel {};
        return View(model);
    }
    catch (Exception ex)
    {
        TempData["ErrorMessage"] = $"Hata: {ex.Message}";
        ViewBag.AllCategories = new List<CategoryDto>();
        return View(new CreateProductViewModel());
    }
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
            // Şirket ID'sini JWT Claim'den veya Session'dan alıyoruz (Kritik!)
            CompanyId = Guid.Parse(User.FindFirstValue("CompanyId") ?? Guid.Empty.ToString())
        };

        var response = await _apiService.PostAsync<ProductCreateDto, Guid>("Product/Create", productDto);

        if (response != null && response.Success)
        {
            TempData["SuccessMessage"] = "Ürün başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = response?.Message ?? "Ürün eklenirken bir hata oluştu.";
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