using AutoMapper;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
        return ApiResponse<IEnumerable<UserDto>>.SuccessResult(userDtos);
    }

    public async Task<ApiResponse<IEnumerable<UserDto>>> GetCompanyStaffAsync(Guid companyId)
    {
        // Şirkete ait silinmemiş personelleri getir
        var staff = await _unitOfWork.Users.FindAsync(u => u.CompanyId == companyId && !u.IsDeleted);
        var dtos = _mapper.Map<IEnumerable<UserDto>>(staff);
        return ApiResponse<IEnumerable<UserDto>>.SuccessResult(dtos);
    }

    public async Task<ApiResponse<bool>> UpdateProfileAsync(Guid userId, UserUpdateDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return ApiResponse<bool>.ErrorResult("Kullanıcı bulunamadı.");

        // Bilgileri güncelle
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Profil başarıyla güncellendi.");
    }

    public async Task<ApiResponse<bool>> AssignRoleAsync(UserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(dto.Id);
        if (user == null) return ApiResponse<bool>.ErrorResult("Kullanıcı bulunamadı.");

        user.Role = dto.Role;
        user.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, $"Kullanıcıya '{dto.Role}' rolü atandı.");
    }

    public async Task<ApiResponse<bool>> RemoveRoleAsync(UserDto dto)
    {
        if (!Guid.TryParse(dto.Id.ToString(), out Guid userGuid))
            return ApiResponse<bool>.ErrorResult("Geçersiz Kullanıcı ID formatı.");

        var user = await _unitOfWork.Users.GetByIdAsync(userGuid);
        if (user == null) return ApiResponse<bool>.ErrorResult("Kullanıcı bulunamadı.");

        // Rolü varsayılan "Customer" seviyesine çek
        user.Role = "Customer";
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Kullanıcı rolü 'Customer' olarak sıfırlandı.");
    }
    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return ApiResponse<UserDto>.ErrorResult("Kullanıcı bulunamadı.");

        var userDto = _mapper.Map<UserDto>(user);
        return ApiResponse<UserDto>.SuccessResult(userDto);
    }

    
}