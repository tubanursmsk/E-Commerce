using ECommerce.Application.DTOs.Dashboard;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }


    [HttpGet("Stats")]
    public async Task<IActionResult> GetStats()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var companyIdStr = User.FindFirstValue("companyId");
        Guid? companyId = string.IsNullOrEmpty(companyIdStr) ? null : Guid.Parse(companyIdStr);

        return Ok(await _dashboardService.GetStatsAsync(companyId, role ?? ""));
    }
}

