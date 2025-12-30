using AutoMapper;
using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AdminService(IUnitOfWork unitOfWork, IMapper mapper)
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

    public async Task<ApiResponse<bool>> AssignRoleAsync(RoleDto dto)
    {
        // UserId üzerinden kullanıcıyı çekiyoruz (BaseEntity Guid olduğu için Guid'e çeviriyoruz)
        if (!Guid.TryParse(dto.UserId.ToString(), out Guid userGuid))
            return ApiResponse<bool>.ErrorResult("Geçersiz Kullanıcı ID formatı.");

        var user = await _unitOfWork.Users.GetByIdAsync(userGuid);
        if (user == null) return ApiResponse<bool>.ErrorResult("Kullanıcı bulunamadı.");

        // Rolü güncelle (Senin User entity'ndeki Role string alanına yazar)
        user.Role = dto.RoleName;
        user.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, $"Kullanıcıya '{dto.RoleName}' rolü başarıyla atandı.");
    }

    public async Task<ApiResponse<bool>> RemoveRoleAsync(RoleDto dto)
    {
        if (!Guid.TryParse(dto.UserId.ToString(), out Guid userGuid))
            return ApiResponse<bool>.ErrorResult("Geçersiz Kullanıcı ID formatı.");

        var user = await _unitOfWork.Users.GetByIdAsync(userGuid);
        if (user == null) return ApiResponse<bool>.ErrorResult("Kullanıcı bulunamadı.");

        // Rolü varsayılan "Customer" seviyesine çek
        user.Role = "Customer";
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Kullanıcı rolü 'Customer' olarak sıfırlandı.");
    }
}