using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Models.Brands;
using ECommerce.AdminPanel.Models.Products;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.DTOs.Customer;
using ECommerce.Application.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

[Authorize]
public class CustomerController : Controller
{

    private readonly BaseApiService _apiService;

    public CustomerController(BaseApiService apiService)
    {
        _apiService = apiService;
    }


        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<IEnumerable<CustomerDto>>("Customer");
            return View(response?.Data ?? new List<CustomerDto>());
        }
    

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        // Müşteriyi pasife çekme veya aktif etme işlemi
        var response = await _apiService.PostAsync<object, bool>($"User/ToggleStatus/{id}", new { });

        if (response != null && response.Success)
            TempData["SuccessMessage"] = "Müşteri durumu güncellendi.";

        return RedirectToAction(nameof(Index));
    }
}