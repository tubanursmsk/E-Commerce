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
}
/*
Mülakat Notu: "Custom Repository Metodu"
Mülakatta sana "Neden Generic Repository dışına çıkıp özel metot yazdın?" 

"Generic Repository temel CRUD işlemleri için yeterli olsa da, ilişkili tabloların (Include) ve özel iş kurallarının
 (Company bazlı filtreleme) gerektiği durumlarda mimariyi bozmadan Specific Repository (Örn: ProductRepository) kullandım. 
 Böylece servis katmanını karmaşık LINQ sorgularından arındırıp iş mantığına odaklanmasını sağladım."

*/