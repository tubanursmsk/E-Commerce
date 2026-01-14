using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Infrastructure.Services;
using System.Security.Claims;

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

}