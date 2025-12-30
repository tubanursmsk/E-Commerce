using AutoMapper;
using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Helpers;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<string>> LoginAsync(LoginDto dto)
    {
        // 1. Kullanıcıyı bul
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == dto.Email);
        var user = users.FirstOrDefault();

        if (user == null) 
            return ApiResponse<string>.ErrorResult("E-posta veya şifre hatalı.");

        // 2. Şifreyi doğrula
        if (!PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            return ApiResponse<string>.ErrorResult("E-posta veya şifre hatalı.");

        // 3. Token Üret (JwtTokenHelper kullanarak)
        // Not: User tablosunda CompanyId Guid? olduğu için boşsa Guid.Empty gönderiyoruz
        var token = JwtTokenHelper.GenerateToken(
            user.Id, 
            user.Email, 
            user.CompanyId ?? Guid.Empty, 
            new List<string> { user.Role }
        );

        return ApiResponse<string>.SuccessResult(token, "Giriş başarılı.");
    }

   public async Task<ApiResponse<Guid>> RegisterWithCompanyAsync(RegisterCompanyDto dto)
{
    // 1. Email kontrolü
    var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == dto.Email);
    if (existingUser.Any()) return ApiResponse<Guid>.ErrorResult("Email zaten kayıtlı.");

    // 2. ÖNCE ŞİRKETİ OLUŞTUR
    var newCompany = new Company 
    { 
        Id = Guid.NewGuid(),
        Name = dto.CompanyName,
        Phone = dto.Phone,
        TaxNumber = dto.TaxNumber,
        City = dto.City,
        District = dto.District,
        FullAddress = dto.FullAddress,
        Status = true
    };
    await _unitOfWork.Companies.AddAsync(newCompany);

    // 3. KULLANICIYI OLUŞTUR VE ŞİRKETİ BAĞLA
    var user = new User
    {
        Id = Guid.NewGuid(),
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        PasswordHash = PasswordHasher.HashPassword(dto.Password),
        Role = "CompanyManager", // Kayıt olan kişi artık Customer değil Manager
        CompanyId = newCompany.Id // İşte kritik nokta burası!
    };

    await _unitOfWork.Users.AddAsync(user);
    
    // UnitOfWork sayesinde ikisi birden aynı anda kaydedilir (Transaction)
    await _unitOfWork.SaveChangesAsync();

    return ApiResponse<Guid>.SuccessResult(user.Id, "Şirket ve yönetici kaydı başarılı.");
}
}
