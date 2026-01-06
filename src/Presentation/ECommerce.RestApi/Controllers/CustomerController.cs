using ECommerce.Application.DTOs.Customer;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommerce.Application.Responses;

namespace ECommerce.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    public CustomerController(ICustomerService customerService) => _customerService = customerService;



    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var companyIdStr = User.FindFirstValue("companyId");
        Guid? companyId = string.IsNullOrEmpty(companyIdStr) ? null : Guid.Parse(companyIdStr);

        var result = await _customerService.GetAllAsync(companyId, role);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create(CustomerCreateDto dto) => Ok(await _customerService.CreateAsync(dto));


    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        var result = await _customerService.SearchAsync(keyword);
        return Ok(result);
    }
}