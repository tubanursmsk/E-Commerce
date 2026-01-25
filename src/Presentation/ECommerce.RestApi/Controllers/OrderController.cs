using System.Security.Claims;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces;
using ECommerce.Infrastructure.Services;
using ECommerce.RestApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Sipariş işlemleri için giriş zorunlu
//[ApiKey]    // X-Api-Key zorunlu
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("List")]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var companyIdStr = User.FindFirstValue("companyId");
        Guid? companyId = string.IsNullOrEmpty(companyIdStr) ? null : Guid.Parse(companyIdStr);

        // OrderService içinde yeni bir metot veya filtreleme ekleyelim
        var result = await _orderService.GetAllFilteredAsync(companyId, role ?? "");
        return Ok(result);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(OrderCreateDto dto)
    {
        var result = await _orderService.CreateOrderAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _orderService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPatch("UpdateStatus/{id}")]
    [Authorize(Roles = "Admin")] // Sadece Admin sipariş durumunu değiştirebilir
    public async Task<IActionResult> UpdateStatus(Guid id, ECommerce.Domain.Enums.OrderStatus status)
    {
        var result = await _orderService.UpdateStatusAsync(id, status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("Search")]
    public async Task<IActionResult> SearchByNumber([FromQuery] string orderNumber)
    {
        var result = await _orderService.SearchByOrderNumberAsync(orderNumber);
        return Ok(result);
    }

    [HttpGet("ByCustomer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var companyIdStr = User.FindFirstValue("companyId");
        Guid? companyId = string.IsNullOrEmpty(companyIdStr) ? null : Guid.Parse(companyIdStr);

        // Müşteri kendi siparişlerine bakıyorsa CompanyId filtresini kaldırıyoruz
        if (role == "Customer")
        {
            companyId = null;
        }
        var result = await _orderService.GetByCustomerIdAsync(customerId, companyId, role ?? "");
        return Ok(result);
    }
}