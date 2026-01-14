using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ECommerce.Application.DTOs.Dashboard;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DashboardStatsDto>> GetStatsAsync(Guid? companyId, string role)
    {
        var stats = new DashboardStatsDto();
        var lastMonth = DateTime.UtcNow.AddDays(-30);

        // 1. Bekleyen Sipariş Sayısı
        stats.PendingOrdersCount = await _unitOfWork.Orders.CountAsync(o =>
            o.Status == OrderStatus.Pending &&
            (!companyId.HasValue || o.CompanyId == companyId));

        // 2. Kritik Stoklar (Stok < 10 olan ürünler)
        stats.CriticalStockCount = await _unitOfWork.Products.CountAsync(p =>
            p.Stock < 10 &&
            (!companyId.HasValue || p.CompanyId == companyId));

        // 3. Toplam Müşteri Sayısı (Role Bazlı)
        if (role == "Admin")
        {
            // Admin sistemdeki tüm silinmemiş müşterileri görür
            stats.TotalCustomersCount = await _unitOfWork.Customers.CountAsync(c => !c.IsDeleted);
        }
        else
        {
            // Şirket yöneticisi sadece kendi şirketinden sipariş vermiş tekil müşterileri görür
            var companyOrders = await _unitOfWork.Orders.FindAsync(o => o.CompanyId == companyId);
            stats.TotalCustomersCount = companyOrders.Select(o => o.CustomerId).Distinct().Count();
        }

        // 4. Aylık Toplam Satış (Son 30 Gün)
        var monthlyOrders = await _unitOfWork.Orders.FindAsync(o =>
            o.CreatedDate >= lastMonth &&
            o.Status != OrderStatus.Cancelled &&
            (!companyId.HasValue || o.CompanyId == companyId));

        stats.MonthlyTotalSales = monthlyOrders.Sum(o => o.TotalAmount);

        // 5. En Çok Satan 5 Ürün (GroupBy Mantığı)
        // Not: Bu metodun çalışması için OrderRepository içinde GetAllWithItemsAsync tanımlı olmalıdır.
        var ordersWithItems = await _unitOfWork.Orders.GetAllWithItemsAsync(companyId);

        if (ordersWithItems != null)
        {
            stats.TopProducts = ordersWithItems
                .SelectMany(o => o.OrderItems)
                .GroupBy(i => i.Product.Name)
                .Select(g => new TopProductDto
                {
                    ProductName = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToList();
        }

        return ApiResponse<DashboardStatsDto>.SuccessResult(stats, "İstatistikler başarıyla getirildi.");
    }
}