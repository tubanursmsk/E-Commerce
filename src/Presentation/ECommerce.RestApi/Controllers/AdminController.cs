using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Sadece Admin yetkisi olanlar bu controller'ı görebilir
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("User List")]
    public async Task<IActionResult> GetUsers() => Ok(await _adminService.GetAllUsersAsync());

    [HttpPost("Assign Role")]
    public async Task<IActionResult> AssignRole(RoleDto dto) => Ok(await _adminService.AssignRoleAsync(dto));

    [HttpPost("Remove Role")]
    public async Task<IActionResult> RemoveRole(RoleDto dto) => Ok(await _adminService.RemoveRoleAsync(dto));
}