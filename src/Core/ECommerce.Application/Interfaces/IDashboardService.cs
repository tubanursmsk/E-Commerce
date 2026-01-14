using ECommerce.Application.DTOs.Dashboard;
using ECommerce.Application.Responses;
public interface IDashboardService
{
    Task<ApiResponse<DashboardStatsDto>> GetStatsAsync(Guid? companyId, string role);
}