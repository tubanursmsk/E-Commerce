using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Infrastructure.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Application.DTOs.Customer;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Login")] //async de hata olursa hocanın yaptığına bak
    public async Task<IActionResult> Login(LoginDto userLogindto)
    {
        var authResponseDto = await _authService.LoginAsync(userLogindto);
        if (authResponseDto == null)
        {
            return Unauthorized("Email or password is incorrect");
        }
        return Ok(authResponseDto);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterCompanyDto userRegisterCompanyDto)
    {
        var user = await _authService.RegisterWithCompanyAsync(userRegisterCompanyDto);
        return Ok(user);
    }

    [HttpPost("UserRegister")]
    public async Task<IActionResult> UserRegister(RegisterDto userRegisterDto)
    {
        var companyId = Guid.Parse(User.FindFirstValue("companyId"));
        var user = await _authService.RegisterForCompanyAsync(userRegisterDto, companyId);
        return Ok(user);
    }

    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        // Token'daki kullanıcı ID'si ile DTO'daki ID'nin eşleştiğinden emin olalım (Güvenlik)
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (dto.UserId != currentUserId) return Forbid();

        var result = await _authService.ChangePasswordAsync(dto);
        return Ok(result);
    }

    [HttpPost("RegisterCustomer")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCustomer(RegisterDto dto)
    {
        // RegisterForCompanyAsync yerine yeni bir metod yazıyoruz
        var result = await _authService.RegisterCustomerAsync(dto);
        return Ok(result);
    }

    
    

}