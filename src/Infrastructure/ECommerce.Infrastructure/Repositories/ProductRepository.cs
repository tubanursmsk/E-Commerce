using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Infrastructure.Repositories;  // Buraya Product'a özel (Join'li sorgular vb.) metodlar gelecek.

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private new readonly AppDbContext _context;
    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    // Repository içindeki örnek sorgu
    public async Task<IEnumerable<Product>> GetAllWithCategoryAndBrandAsync()
    {
        return await _context.Products
            .Include(p => p.Category) // Kategori bilgilerini bağla
            .Include(p => p.Brand)    // Marka bilgilerini bağla
            .Where(p => !p.IsDeleted) // Eğer Soft Delete kullanıyorsan
            .ToListAsync();
    }

    /*
    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()

    {
        return await _context.Products
            .Include(p => p.Category) // Category tablosunu bağla
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithBrandAsync()

    {
        return await _context.Products
            .Include(p => p.Brand) // Brand tablosunu bağla
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }*/
}