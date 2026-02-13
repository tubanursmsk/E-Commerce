using ECommerce.AdminPanel.Models;
using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AdminPanel.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly BaseApiService _apiService;
    public ReviewController(BaseApiService apiService) => _apiService = apiService;

    public async Task<IActionResult> Index()
    {
        var response = await _apiService.GetAsync<IEnumerable<ReviewDto>>("Review/ListAll");
        return View(response?.Data ?? new List<ReviewDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _apiService.DeleteAsync($"Review/Delete/{id}");
        if (response != null && response.Success)
            TempData["SuccessMessage"] = "Yorum başarıyla silindi.";
        return RedirectToAction(nameof(Index));
    }
}