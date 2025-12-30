using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Infrastructure.Services;

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
    public async Task<IActionResult> Register(RegisterCompanyDto userRegisterdto)
    {
        var user = await _authService.RegisterWithCompanyAsync(userRegisterdto);
        return Ok(user);
    }
}