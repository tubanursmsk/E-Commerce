using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        // Özel sınıfları olanlar
        Products = new ProductRepository(_context);
        Orders = new OrderRepository(_context);
        Customers = new CustomerRepository(_context);
        Companies = new CompanyRepository(_context);
        
        
        // Özel sınıfı olmayan, genel işlemleri kullananlar
        Categories = new GenericRepository<Category>(_context);
        Brands = new GenericRepository<Brand>(_context);
        Reviews = new GenericRepository<Review>(_context);
        Banners = new GenericRepository<Banner>(_context);
        Users = new GenericRepository<User>(_context);
        Roles = new GenericRepository<Role>(_context);
        Requests = new GenericRepository<Request>(_context);
        Cargoes = new GenericRepository<Cargo>(_context);
    }

    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public ICustomerRepository Customers { get; }
    public ICompanyRepository Companies { get; }
    public IGenericRepository<Category> Categories { get; }
    public IGenericRepository<Brand> Brands { get; }
    public IGenericRepository<Review> Reviews { get; }
    public IGenericRepository<Banner> Banners { get; }
    public IGenericRepository<User> Users { get; }
    public IGenericRepository<Role> Roles { get; }
    public IGenericRepository<Request> Requests { get; }
    public IGenericRepository<Cargo> Cargoes { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

/*2. UnitOfWork (Gerçek İşçi)
Nerede: Infrastructure katmanında. Görevi: IUnitOfWork planına sadık kalarak gerçek işi yapar.
 AppDbContext'i (yani veritabanı bağlantısını) sadece bu dosya tanır.

Neden gerekli? Veritabanına fiziksel olarak dokunan, SaveChangesAsync komutunu gerçekten SQL'e 
gönderen yer burasıdır.*/