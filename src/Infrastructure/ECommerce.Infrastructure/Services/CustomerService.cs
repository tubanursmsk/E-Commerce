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

    public async Task<ApiResponse<CustomerDto>> GetByIdAsync(Guid id)
    {
        // User tablosunu dahil eden repository metodunu çağırıyoruz
        var customer = await _unitOfWork.Customers.GetByIdWithUserAsync(id);

        if (customer == null) return ApiResponse<CustomerDto>.ErrorResult("Müşteri bulunamadı.");

        return ApiResponse<CustomerDto>.SuccessResult(_mapper.Map<CustomerDto>(customer));
    }

    public async Task<ApiResponse<Guid>> CreateAsync(CustomerCreateDto dto)
    {
        var customer = _mapper.Map<Customer>(dto);
        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<Guid>.SuccessResult(customer.Id, "Müşteri profili oluşturuldu.");
    }

    // Interface ile tam uyumlu hale getirildi
    public async Task<ApiResponse<IEnumerable<CustomerDto>>> GetAllAsync(Guid? currentCompanyId, string role)
    {
        IEnumerable<Customer> customers;
        if (role == "Admin")
        {
            customers = await _unitOfWork.Customers.GetAllWithUserAsync();
        }
        else if (role == "CompanyManager" && currentCompanyId.HasValue)
        {
            customers = await _unitOfWork.Customers.GetCustomersByCompanyIdAsync(currentCompanyId.Value);
        }
        else
        {
            return ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(new List<CustomerDto>());
        }
        var dtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
        return ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(dtos);
    }

    public async Task<ApiResponse<IEnumerable<CustomerDto>>> SearchAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            var all = await _unitOfWork.Customers.GetAllWithUserAsync();
            return ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(_mapper.Map<IEnumerable<CustomerDto>>(all));
        }

        var customers = await _unitOfWork.Customers.FindWithUserAsync(c =>
            c.User.FirstName.ToLower().Contains(keyword.ToLower()) ||
            c.User.LastName.ToLower().Contains(keyword.ToLower()) ||
            c.Phone.Contains(keyword));

        if (customers == null || !customers.Any())
        {
            return ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(new List<CustomerDto>(), "Müşteri bulunamadı.");
        }

        var dtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
        return ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(dtos);
    }

    public async Task<ApiResponse<bool>> UpdateProfileAsync(CustomerUpdateDto dto)
{
    // 1. Önce User tablosundaki Ad, Soyad ve Email'i güncelle
    var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
    if (user == null) return ApiResponse<bool>.ErrorResult("Kullanıcı bulunamadı.");

    user.FirstName = dto.FirstName;
    user.LastName = dto.LastName;
    user.Email = dto.Email; // Email değiştirmek genelde ekstra doğrulama ister, şimdilik kapalı tutabiliriz veya açabiliriz.
    
    _unitOfWork.Users.Update(user);

    // 2. Şimdi Customer tablosundaki Telefon, Adres, Şehir bilgilerini güncelle
    // Kullanıcının ID'sine bağlı bir Müşteri kaydı var mı diye bakıyoruz
    var customer = (await _unitOfWork.Customers.FindAsync(c => c.UserId == dto.UserId)).FirstOrDefault();

    if (customer == null)
    {
        // Eğer müşteri kaydı yoksa (İlk kez profil dolduruyor), YENİ OLUŞTUR
        customer = new Customer
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            Status = true,
            CreatedDate = DateTime.UtcNow
        };
        await _unitOfWork.Customers.AddAsync(customer);
    }
    else
    {
        // Varsa GÜNCELLE
        customer.Phone = dto.Phone;
        customer.Address = dto.Address;
        customer.City = dto.City;
        customer.UpdatedDate = DateTime.UtcNow;
        _unitOfWork.Customers.Update(customer);
    }

    await _unitOfWork.SaveChangesAsync();
    return ApiResponse<bool>.SuccessResult(true, "Profil bilgileri başarıyla güncellendi.");
}

// Profil sayfasını açtığında verileri doldurmak için:
public async Task<ApiResponse<CustomerDto>> GetProfileByUserIdAsync(Guid userId)
{
    // Kullanıcıya ait müşteri kaydını bul
    var customer = (await _unitOfWork.Customers.GetByIdWithUserAsync(userId)); // Repository'de UserId'ye göre getiren metod yoksa FindAsync kullanacağız:
    
    // Eğer repository'de özel metod yoksa:
    // var customer = (await _unitOfWork.Customers.FindWithUserAsync(c => c.UserId == userId)).FirstOrDefault();
    
    if (customer == null)
    {
        // Müşteri kaydı yoksa bile User bilgilerini dönmeliyiz ki form dolsun
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if(user == null) return ApiResponse<CustomerDto>.ErrorResult("Kullanıcı bulunamadı");

        return ApiResponse<CustomerDto>.SuccessResult(new CustomerDto 
        { 
            FirstName = user.FirstName, 
            LastName = user.LastName, 
            Email = user.Email,
            UserId = user.Id
            // Telefon ve Adres boş dönecek
        });
    }

    return ApiResponse<CustomerDto>.SuccessResult(_mapper.Map<CustomerDto>(customer));
}
}