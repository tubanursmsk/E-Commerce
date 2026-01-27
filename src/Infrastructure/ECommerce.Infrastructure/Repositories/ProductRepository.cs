
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
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
            .Include(p => p.Company)  // Şirket bilgilerini bağla
            .Where(p => !p.IsDeleted) // Eğer Soft Delete kullanıyorsan
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCompanyIdListAsync(Guid companyId)
    {
        return await _context.Products
            .AsNoTracking() // .AsNoTracking() harika bir detay. Sadece listeleme yapacağımız (güncelleme yapmayacağımız) verilerde EF Core'un takip mekanizmasını kapatmak bellek kullanımını azaltır ve hızı artırır.
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Company)
            .Where(p => !p.IsDeleted && p.CompanyId == companyId)  // ✅ FİLTRE BURADA, Filtreyi (p.CompanyId == companyId) doğrudan veritabanı sorgusuna (SQL) gömdüğün için, veriler API'ye gelmeden önce filtrelenmiş oldu. Bu en güvenli yaklaşımdır.
            .ToListAsync();
    }

    /*
    Mülakat Notu: "Custom Repository Metodu"
    Mülakatta sana "Neden Generic Repository dışına çıkıp özel metot yazdın?" 

    "Generic Repository temel CRUD işlemleri için yeterli olsa da, ilişkili tabloların (Include) ve özel iş kurallarının
     (Company bazlı filtreleme) gerektiği durumlarda mimariyi bozmadan Specific Repository (Örn: ProductRepository) kullandım. 
     Böylece servis katmanını karmaşık LINQ sorgularından arındırıp iş mantığına odaklanmasını sağladım."

    */

public async Task<(IEnumerable<Product> Items, int TotalCount)> GetFilteredAsync(ProductFilterParams filter)
{
    var query = _context.Products
        .AsNoTracking() // Listeleme olduğu için performans artırır
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Include(p => p.Company)
        .Where(p => !p.IsDeleted)
        .AsQueryable();

    // 1. Kategori
    if (filter.CategoryId.HasValue)
        query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

    // 2. Marka
    if (filter.BrandIds != null && filter.BrandIds.Any())
        query = query.Where(p => filter.BrandIds.Contains(p.BrandId));

    // 3. Fiyat
    if (filter.MinPrice.HasValue)
        query = query.Where(p => p.Price >= filter.MinPrice.Value);
    
    if (filter.MaxPrice.HasValue)
        query = query.Where(p => p.Price <= filter.MaxPrice.Value);

    // 4. Arama
    if (!string.IsNullOrWhiteSpace(filter.Keyword))
    {
        var lowerKeyword = filter.Keyword.ToLower();
        query = query.Where(p => p.Name.ToLower().Contains(lowerKeyword) || 
                                 (p.Description != null && p.Description.ToLower().Contains(lowerKeyword)));
    }

    // 5. Özellikler
    if (filter.IsFreeShipping.HasValue && filter.IsFreeShipping.Value)
        query = query.Where(p => p.IsFreeShipping);

    if (filter.IsFastDelivery.HasValue && filter.IsFastDelivery.Value)
        query = query.Where(p => p.IsFastDelivery);

    // 6. Sıralama
    // SQLite için decimal alanlarda (double) dönüşümü yapıyoruz sebeb: SQLite, decimal tipini yerel olarak desteklemez, genellikle double veya string olarak saklar.
    query = filter.SortBy switch
    {
        "price_asc" => query.OrderBy(p => p.Price),
        "price_desc" => query.OrderByDescending(p => p.Price),
        "name_asc" => query.OrderBy(p => p.Name),
        "newest" => query.OrderByDescending(p => p.CreatedDate),
        _ => query.OrderBy(p => p.Name)
    };

    var totalCount = await query.CountAsync();

    var items = await query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync();

    return (items, totalCount);
}

}