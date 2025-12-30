using AutoMapper;
using ECommerce.Application.DTOs.Customer;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<Guid>> CreateAsync(CustomerCreateDto dto)
    {
        var customer = _mapper.Map<Customer>(dto);
        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<Guid>.SuccessResult(customer.Id, "Müşteri profili oluşturuldu.");
    }

    public async Task<ApiResponse<IEnumerable<CustomerDto>>> GetAllAsync()
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();
        return ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(_mapper.Map<IEnumerable<CustomerDto>>(customers));
    }

    public async Task<ApiResponse<IEnumerable<CustomerDto>>> SearchAsync(string keyword)
{
    if (string.IsNullOrWhiteSpace(keyword))
    {
        var all = await _unitOfWork.Customers.GetAllWithUserAsync();
        return ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(_mapper.Map<IEnumerable<CustomerDto>>(all));
    }

    // Yeni yazdığımız 'WithUser' metodunu çağırıyoruz
    var customers = await _unitOfWork.Customers.FindWithUserAsync(c => 
        c.User.FirstName.ToLower().Contains(keyword.ToLower()) || 
        c.User.LastName.ToLower().Contains(keyword.ToLower()) ||
        c.PhoneNumber.Contains(keyword));

    if (customers == null || !customers.Any())
    {
        return ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(new List<CustomerDto>(), "Müşteri bulunamadı.");
    }

    var dtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
    return ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(dtos);
}
}