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
using ClosedXML.Excel;

namespace ECommerce.AdminPanel.Controllers;

[Authorize] 
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
        var response = await _apiService.GetAsync<IEnumerable<ProductDto>>("Product/List");

        var model = new ProductListViewModel
        {
            Products = new PagedResult<ProductDto> 
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
        // 1. Kategorileri Çekince API'den veri gelmezse boş liste gönderelim ki View patlamasın
        var categoryResponse = await _apiService.GetAsync<IEnumerable<CategoryDto>>("Category/List");
        ViewBag.AllCategories = categoryResponse?.Data?.ToList() ?? new List<CategoryDto>();

        // 2. Markaları Çek 
        var brandResponse = await _apiService.GetAsync<IEnumerable<BrandDto>>("Brand/List"); // API endpoint'inin doğruluğundan emin ol
        ViewBag.AllBrands = brandResponse?.Data?.ToList() ?? new List<BrandDto>();

        if ((categoryResponse != null && !categoryResponse.Success) || (brandResponse != null && !brandResponse.Success))
        {
            TempData["ErrorMessage"] = "Veriler yüklenirken bir sorun oluştu.";
        }

        // CompanyId'yi Claims veya Session'dan alarak ViewModel'e atayalım 
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
        await LoadViewBags(); // Krtik Nokta: Listeleri tekrar yüklemezsek "Value cannot be null" hatası alırız çünkü dropdown'lar boş kalır.

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

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var productResponse = await _apiService.GetAsync<ProductDto>($"Product/GetById/{id}");
        if (productResponse == null || !productResponse.Success)
        {
            TempData["ErrorMessage"] = "Ürün bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var product = productResponse.Data;

        // ViewBags (Kategori/Marka)
        await LoadViewBags();

        var model = new UpdateProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description ?? string.Empty,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            CompanyId = product.CompanyId,
            ExistingImages = product.Images ?? new List<string>()
        };

        // Eğer Images listesi boşsa ama ImageUrl doluysa onu ekle (Eski kayıtlar için)
        if (!model.ExistingImages.Any() && !string.IsNullOrEmpty(product.ImageUrl))
        {
            model.ExistingImages.Add(product.ImageUrl);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewBags();
            return View(model);
        }

        // --- MULTIPART FORM DATA HAZIRLIĞI ---
        using var content = new MultipartFormDataContent();

        // Text Alanları
        content.Add(new StringContent(model.Name), nameof(ProductUpdateDto.Name));
        content.Add(new StringContent(model.Description ?? ""), nameof(ProductUpdateDto.Description));
        content.Add(new StringContent(model.Price.ToString()), nameof(ProductUpdateDto.Price));
        content.Add(new StringContent(model.Stock.ToString()), nameof(ProductUpdateDto.Stock));
        content.Add(new StringContent(model.CategoryId.ToString()), nameof(ProductUpdateDto.CategoryId));
        content.Add(new StringContent(model.BrandId.ToString()), nameof(ProductUpdateDto.BrandId));
        content.Add(new StringContent(model.CompanyId.ToString()), nameof(ProductUpdateDto.CompanyId));

        // YENİ: Dosyaları Ekle
        if (model.Files != null && model.Files.Count > 0)
        {
            foreach (var file in model.Files)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                // Backend DTO'daki isim "ImageFiles" olmalı
                content.Add(fileContent, "ImageFiles", Path.GetFileName(file.FileName));
            }
        }

        // PUT İsteği
        var response = await _apiService.PutMultipartAsync<bool>($"Product/Update/{model.Id}", content);

        if (response != null && response.Success)
        {
            TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        TempData["ErrorMessage"] = response?.Message ?? "Güncelleme sırasında hata oluştu.";
        await LoadViewBags();
        return View(model);
    }

    // ÜRÜN SİLME
    [HttpPost] 
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
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

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        // 1. Verileri API'den Çek (Tüm listeyi istiyoruz)
        var response = await _apiService.GetAsync<IEnumerable<ProductDto>>("Product/List");
        var products = response?.Data?.ToList() ?? new List<ProductDto>();

        // 2. Excel Dosyasını Oluştur (ClosedXML)
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Ürün Listesi");

            // --- Başlık Satırı ---
            worksheet.Cell(1, 1).Value = "Ürün Adı";
            worksheet.Cell(1, 2).Value = "Marka";
            worksheet.Cell(1, 3).Value = "Kategori";
            worksheet.Cell(1, 4).Value = "Fiyat";
            worksheet.Cell(1, 5).Value = "Stok";
            worksheet.Cell(1, 6).Value = "Oluşturulma Tarihi";

            // Başlıkları Kalın yapıp ve Arka Plan Rengi Ver
            var headerRange = worksheet.Range("A1:F1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // --- Verileri Doldur ---
            int row = 2;
            foreach (var item in products)
            {
                worksheet.Cell(row, 1).Value = item.Name;
                worksheet.Cell(row, 2).Value = item.BrandName ?? "-";
                worksheet.Cell(row, 3).Value = item.CategoryName ?? "-";
                worksheet.Cell(row, 4).Value = item.Price; 
                worksheet.Cell(row, 5).Value = item.Stock;
                row++;
            }

            // --- Sütun Genişliklerini Otomatik Ayarla ---
            worksheet.Columns().AdjustToContents();

            // --- Dosyayı İndir ---
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                // Dosya adı: Urunler_10022026.xlsx
                var fileName = $"Urunler_{DateTime.Now:ddMMyyyy}.xlsx";

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}