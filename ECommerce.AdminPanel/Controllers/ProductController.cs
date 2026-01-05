using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Models.Products; 
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