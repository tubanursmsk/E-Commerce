using ECommerce.AdminPanel.Models.Cargo;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Cargo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

[Authorize]
public class CargoController : Controller
{
    private readonly BaseApiService _apiService;

    public CargoController(BaseApiService apiService)
    {
        _apiService = apiService;
    }

    // KARGO LİSTESİ
    public async Task<IActionResult> Index()
    {
        var response = await _apiService.GetAsync<IEnumerable<CargoDto>>("Cargo");
        var model = new CargoListViewModel { Cargoes = response?.Data ?? new List<CargoDto>() };
        return View(model);
    }

    [HttpGet]
    public IActionResult Create() => View(new CargoViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CargoViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var companyIdStr = User.FindFirstValue("companyId") ?? HttpContext.Session.GetString("companyId");
        model.CompanyId = Guid.Parse(companyIdStr ?? Guid.Empty.ToString());

        var response = await _apiService.PostAsync<CargoViewModel, Guid>("Cargo/Create", model);

        if (response != null && response.Success)
        {
            TempData["SuccessMessage"] = "Kargo firması başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = response?.Message;
        return View(model);
    }

    

    [HttpGet]
public async Task<IActionResult> Update(Guid id)
{
    // Rota: api/Cargo/{id} çağrısı yapar
    var response = await _apiService.GetAsync<CargoDto>($"Cargo/GetById/{id}");
    
    if (response == null || !response.Success || response.Data == null) 
        return NotFound();

    // DTO'yu ViewModel'e manuel veya mapper ile aktaralım
    var model = new UpdateCargoViewModel {
        Id = response.Data.Id,
        Name = response.Data.Name,
        BasePrice = response.Data.BasePrice,
        TrackingUrlPrefix = response.Data.TrackingUrlPrefix,
        Status = response.Data.Status
    };

    return View(model);
}

  [HttpPost]
public async Task<IActionResult> Update(UpdateCargoViewModel model)
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

    var updateDto = new CargoUpdateDto
    {
        
        Name = model.Name,
        BasePrice = model.BasePrice,
        TrackingUrlPrefix = model.TrackingUrlPrefix,
        Status = model.Status,
        CompanyId = model.CompanyId
    };

    var response = await _apiService.PutAsync<CargoUpdateDto, bool>($"Cargo/Update/{model.Id}", updateDto);

    if (response is { Success: true })
    {
        TempData["SuccessMessage"] = "Kargo firması başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    ViewBag.Error = response?.Message ?? "Güncelleme sırasında hata oluştu.";
    return View(model);
}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _apiService.DeleteAsync($"Cargo/Delete/{id}");
        if (response != null && response.Success)
            TempData["SuccessMessage"] = "Kargo firması silindi.";
        else
            TempData["ErrorMessage"] = response?.Message;

        return RedirectToAction(nameof(Index));
    }
}