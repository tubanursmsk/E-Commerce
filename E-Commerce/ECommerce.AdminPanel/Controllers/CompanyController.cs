using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Models.Company;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Company;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

[Authorize]
public class CompanyController : Controller
{
    private readonly BaseApiService _apiService;
    public CompanyController(BaseApiService apiService) => _apiService = apiService;

    public async Task<IActionResult> Index()
    {
        // Eğer Manager ise listeyi görmesine gerek yok, direkt kendi şirketine gitsin
        if (User.IsInRole("CompanyManager"))
        {
            var myId = User.FindFirstValue("companyId");
            return RedirectToAction(nameof(Update), new { id = myId });
        }

        // Admin ise tüm listeyi çek
        var response = await _apiService.GetAsync<IEnumerable<CompanyDto>>("Company/List");
        return View(response?.Data ?? new List<CompanyDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        // Güvenlik: Manager başkasının ID'sini kurcalayamaz
        if (User.IsInRole("CompanyManager") && id.ToString() != User.FindFirstValue("companyId"))
            return Forbid();

        var response = await _apiService.GetAsync<CompanyDto>($"Company/GetById/{id}");
        if (response == null || !response.Success) return NotFound();

        var model = new CompanyUpdateViewModel
        {
            Id = response.Data.Id,
            Name = response.Data.Name,
            TaxNumber = response.Data.TaxNumber, // Readonly olacak
            Phone = response.Data.Phone,
            FullAddress = response.Data.FullAddress,
            Status = response.Data.Status,
            IsApproved = response.Data.IsApproved
        };
        return View(model);
    }


    // SİLME İŞLEMİ
    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _apiService.DeleteAsync($"Company/Delete/{id}");
        if (response != null && response.Success)
            TempData["SuccessMessage"] = "Şirket kaldırıldı.";

        return RedirectToAction(nameof(Index));
    }
}