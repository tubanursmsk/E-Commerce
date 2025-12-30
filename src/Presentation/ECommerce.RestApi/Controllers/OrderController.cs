using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ECommerce.RestApi.Filters;

namespace ECommerce.RestApi.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize] // Sipariş işlemleri için giriş zorunlu
[ApiKey]    // X-Api-Key zorunlu
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
        var result = await _orderService.GetAllAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _orderService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("Search")]
public async Task<IActionResult> SearchByNumber([FromQuery] string orderNumber)
{
    var result = await _orderService.SearchByOrderNumberAsync(orderNumber);
    return Ok(result);
}

    [HttpPost("Create")]
    public async Task<IActionResult> Create(OrderCreateDto dto)
    {
        var result = await _orderService.CreateOrderAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("UpdateStatus/{id}")]
    [Authorize(Roles = "Admin")] // Sadece Admin sipariş durumunu değiştirebilir
    public async Task<IActionResult> UpdateStatus(Guid id, ECommerce.Domain.Enums.OrderStatus status)
    {
        var result = await _orderService.UpdateStatusAsync(id, status);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}