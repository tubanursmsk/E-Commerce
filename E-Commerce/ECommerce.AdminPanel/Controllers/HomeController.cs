using ECommerce.AdminPanel.Services;
using ECommerce.Application.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.AdminPanel.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly BaseApiService _apiService;
    public HomeController(BaseApiService apiService)
    {
        _apiService = apiService;
    }
    public async Task<IActionResult> Index()
    {
        // API'den Dashboard istatistiklerini çekiyoruz
        var response = await _apiService.GetAsync<DashboardStatsDto>("Dashboard/Stats");
        
        // Veri gelmezse boş bir model gönderiyoruz ki sayfa patlamasın
        var model = response?.Data ?? new DashboardStatsDto();
        
        return View(model);
    }
}