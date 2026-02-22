using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Infrastructure.Repositories;  // Buraya Product'a özel (Join'li sorgular vb.) metodlar gelecek.

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    private new readonly AppDbContext _context;
    public ReviewRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetAllWithDetailsAsync(Guid? companyId)
    {
        return await _context.Reviews
            .Include(r => r.Product) // Hangi ürün?
            .Include(r => r.Customer) // Kim yazdı?
                .ThenInclude(c => c.User) // CustomerName (Ad + Soyad) için
            .Where(r => !r.IsDeleted && (!companyId.HasValue || r.Product.CompanyId == companyId.Value))
            .AsNoTracking()
            .ToListAsync();
    }

}