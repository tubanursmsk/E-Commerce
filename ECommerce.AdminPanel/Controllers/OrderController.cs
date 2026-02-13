using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly BaseApiService _apiService;
    public OrderController(BaseApiService apiService)
    {
        _apiService = apiService;
    }
    public async Task<IActionResult> Index()
    {
        var response = await _apiService.GetAsync<IEnumerable<OrderDto>>("Order/List");

        if (response != null && response.Success)
        {
            return View(response.Data);
        }

        return View(new List<OrderDto>());
    }
    public async Task<IActionResult> Details(Guid id)
    {
        var response = await _apiService.GetAsync<OrderDto>($"Order/{id}");

        if (response == null || !response.Success)
            return NotFound();

        return View(response.Data);
    }
}