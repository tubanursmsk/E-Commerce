using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Models.Products;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
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



    /*[HttpGet] //ürünler listelenirken companyıd ile filtrleme bu işlemi restapi tarafında(daha güvenli) yaptığımız için bu yöntem askıda
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var companyIdStr =
            User.FindFirst("CompanyId")?.Value
            ?? HttpContext.Session.GetString("CompanyId");
        if (!Guid.TryParse(companyIdStr, out var companyId) || companyId == Guid.Empty)
        {
            // CompanyId yoksa login/şirket seçimi akışına yönlendir
            TempData["ErrorMessage"] = "Company bilgisi bulunamadı. Lütfen tekrar giriş yapın.";
            return RedirectToAction("Login", "Auth");
        }
        // DİKKAT: {companyId} yazmıyoruz, gerçek değeri koyuyoruz
        var endpoint = $"Product/List/{companyId}";
        var response = await _apiService.GetAsync<IEnumerable<ProductDto>>(endpoint);
        var items = response?.Data?.ToList() ?? new List<ProductDto>();
        var model = new ProductListViewModel
        {
            Products = new PagedResult<ProductDto>
            {
                Items = items,
                TotalCount = items.Count,
                PageNumber = page,
                PageSize = pageSize
            },
            CompanyId = companyId
        };
        return View(model);
    }*/

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
        // --- 1. Validasyon Kontrolü ---
        if (!ModelState.IsValid)
        {
            await LoadViewBags(); // Hata varsa listeleri tekrar yükle
            return View(model);
        }

        // --- 2. Multipart Form Data Hazırlığı ---
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(model.Name ?? ""), nameof(ProductCreateDto.Name));
        content.Add(new StringContent(model.Description ?? ""), nameof(ProductCreateDto.Description));
        content.Add(new StringContent(model.Price.ToString()), nameof(ProductCreateDto.Price));
        content.Add(new StringContent(model.Stock.ToString()), nameof(ProductCreateDto.Stock));
        content.Add(new StringContent(model.CategoryId.ToString()), nameof(ProductCreateDto.CategoryId));
        content.Add(new StringContent(model.BrandId.ToString()), nameof(ProductCreateDto.BrandId));
        content.Add(new StringContent(model.CompanyId.ToString()), nameof(ProductCreateDto.CompanyId));

        // Dosyaları Ekle
        if (model.Files != null && model.Files.Count > 0)
        {
            foreach (var file in model.Files)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                // DİKKAT: Backend DTO'daki isim "ImageFiles" (Çoğul) olmalı
                content.Add(fileContent, "ImageFiles", file.FileName);
            }
        }

        // --- 3. API İsteği ---
        var response = await _apiService.PostMultipartAsync<Guid>("Product/Create", content);

        if (response != null && response.Success)
        {
            TempData["SuccessMessage"] = "Ürün başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // --- 4. Hata Durumu ---
        // API'den hata döndüyse (400, 500 vb.) sayfayı tekrar yükle ama Listeleri de doldur!
        TempData["ErrorMessage"] = response?.Message ?? "API tarafında bir hata oluştu.";
        await LoadViewBags(); // Krtik Nokta: Listeleri tekrar yüklemezsen "Value cannot be null" hatası alırsın.

        return View(model);
    }

    // Kod tekrarını önlemek için yardımcı metod
    private async Task LoadViewBags()
    {
        var categoryResponse = await _apiService.GetAsync<IEnumerable<CategoryDto>>("Category/List");
        var brandResponse = await _apiService.GetAsync<IEnumerable<BrandDto>>("Brand/List");

        ViewBag.AllCategories = categoryResponse?.Data?.ToList() ?? new List<CategoryDto>();
        ViewBag.AllBrands = brandResponse?.Data?.ToList() ?? new List<BrandDto>();
    }

    // ÜRÜN DÜZENLEME (GET)
    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        // 1. Ürün bilgilerini API'den getir
        var productResponse = await _apiService.GetAsync<ProductDto>($"Product/GetById/{id}");
        if (productResponse == null || !productResponse.Success)
        {
            TempData["ErrorMessage"] = "Ürün bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var product = productResponse.Data;

        // 2. Kategori ve Marka listelerini yükle
        var categoryResponse = await _apiService.GetAsync<IEnumerable<CategoryDto>>("Category/List");
        var brandResponse = await _apiService.GetAsync<IEnumerable<BrandDto>>("Brand/List");

        ViewBag.AllCategories = categoryResponse?.Data?.ToList() ?? new List<CategoryDto>();
        ViewBag.AllBrands = brandResponse?.Data?.ToList() ?? new List<BrandDto>();

        // 3. DTO'yu ViewModel'e eşle
        var model = new UpdateProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description ?? string.Empty,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            CompanyId = product.CompanyId
        };

        return View(model);
    }

    // ÜRÜN DÜZENLEME (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateProductViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);


        var updateDto = new ProductUpdateDto
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            Stock = model.Stock,
            CategoryId = model.CategoryId,
            BrandId = model.BrandId,
            CompanyId = model.CompanyId // ✔ modelden geliyor
        };

        var response = await _apiService.PutAsync<ProductUpdateDto, bool>($"Product/Update/{model.Id}", updateDto);//Buraya id ekledik(ve cshtml de hidden input ekledik)

        if (response.Success)
            return RedirectToAction("Index");

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
        var response = await _apiService.DeleteAsync($"Product/Delete/{id}");

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