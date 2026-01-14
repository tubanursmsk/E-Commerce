using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Models.Brands;
using ECommerce.AdminPanel.Models.Products;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.DTOs.Customer;
using ECommerce.Application.DTOs.Order;
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


/*
    public async Task<IActionResult> Details(Guid id)
{
    // 1. Müşteri bilgilerini çek (CustomerController üzerinden GetById endpoint'in olduğunu varsayıyoruz)
    var customerResponse = await _apiService.GetAsync<CustomerDto>($"Customer/{id}");
    
    // 2. Müşterinin bu şirketteki siparişlerini çek
    var ordersResponse = await _apiService.GetAsync<IEnumerable<OrderDto>>($"Order/ByCustomer/{id}");

    if (customerResponse == null || !customerResponse.Success) return NotFound();

    ViewBag.Orders = ordersResponse?.Data ?? new List<OrderDto>();
    return View(customerResponse.Data);
}
*/

public async Task<IActionResult> Details(Guid id)
{
    // 1. Müşteri bilgilerini çek
    var customerResponse = await _apiService.GetAsync<CustomerDto>($"Customer/{id}");
    
    // API'den müşteri gelmediyse veya hata oluştuysa
    if (customerResponse == null || !customerResponse.Success || customerResponse.Data == null)
    {
        TempData["ErrorMessage"] = "Müşteri bilgileri alınamadı.";
        return RedirectToAction(nameof(Index));
    }

    // 2. Müşterinin siparişlerini çek
    var ordersResponse = await _apiService.GetAsync<IEnumerable<OrderDto>>($"Order/ByCustomer/{id}");

    // View'a null gitmemesi için garantiye alıyoruz
    ViewBag.Orders = ordersResponse?.Data ?? new List<OrderDto>();
    
    return View(customerResponse.Data);
}


}