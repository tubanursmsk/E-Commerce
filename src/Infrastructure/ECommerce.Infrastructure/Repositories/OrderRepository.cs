using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private new readonly AppDbContext _context;
    public OrderRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdWithDetailsAsync(Guid customerId, Guid? companyId)
    {
        var query = _context.Orders
            .Include(o => o.Customer)       // Customer tablosunu bağla
            .ThenInclude(c => c.User)   // Customer içindeki User tablosunu bağla (İsim burada)
            .Where(o => o.CustomerId == customerId && !o.IsDeleted);


        // Eğer bir şirket yöneticisi ise sadece kendi şirketinin siparişlerini görsün
        if (companyId.HasValue)
        {
            query = query.Where(o => o.CompanyId == companyId.Value);
        }

        return await query.AsNoTracking().ToListAsync();
    }


    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync(Guid? companyId)
    {
        return await _context.Orders
            .Include(o => o.Customer)           // Önce Müşteri tablosunu bağla
                .ThenInclude(c => c.User)       // Müşterinin içindeki User tablosunu bağla (İsim burada)
            .Where(o => !o.IsDeleted && (!companyId.HasValue || o.CompanyId == companyId.Value))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)           // Sipariş kalemlerini (ürünleri) getir
            .Include(o => o.Customer)             // Müşteriyi getir
                .ThenInclude(c => c.User)         // Müşteri ismini (User tablosundan) getir
            .FirstOrDefaultAsync(o => o.Id == id);
    }


    public async Task<IEnumerable<Order>> GetAllWithItemsAsync(Guid? companyId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => !o.IsDeleted && (!companyId.HasValue || o.CompanyId == companyId))
            .ToListAsync();
    }

}