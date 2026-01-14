using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("All")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers() => Ok(await _userService.GetAllUsersAsync());

    [HttpPost("AssignRole")]
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> AssignRole(UserDto dto) => Ok(await _userService.AssignRoleAsync(dto));

    [HttpPost("RemoveRole")]
    [Authorize(Roles = "Admin")]    
    public async Task<IActionResult> RemoveRole(UserDto dto) => Ok(await _userService.RemoveRoleAsync(dto));

    [HttpGet("CompanyStaff/{companyId}")] // Admin Panel "User/CompanyStaff/{id}" bekliyor
    [Authorize(Roles = "Admin,CompanyManager")]
    public async Task<IActionResult> GetCompanyStaff(Guid companyId) => Ok(await _userService.GetCompanyStaffAsync(companyId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id) => Ok(await _userService.GetUserByIdAsync(id));   

    [HttpPut("UpdateProfile/{userId}")]
    public async Task<IActionResult> UpdateProfile(Guid userId, UserUpdateDto dto) => Ok(await _userService.UpdateProfileAsync(userId, dto));
}