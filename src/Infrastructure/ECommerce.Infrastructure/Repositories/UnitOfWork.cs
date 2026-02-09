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
        Reviews = new ReviewRepository(_context);

        // Özel sınıfı olmayan, genel işlemleri kullananlar

        Brands = new GenericRepository<Brand>(_context);
        Categories = new GenericRepository<Category>(_context);
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
    public IReviewRepository Reviews { get; }
    public IGenericRepository<Category> Categories { get; }
    public IGenericRepository<Brand> Brands { get; }
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


/*

Dispose Nedir?

Dispose() =
kullanılan kaynağı serbest bırak / işi bitti, kapat demektir.

Burada hangi kaynak var?
private readonly AppDbContext _context;


AppDbContext içinde:
✔ Database connection
✔ Transaction
✔ Memory
✔ Tracking mekanizması tutulur.

İşi bitince kapatılmazsa:
❌ connection açık kalır
❌ memory şişer
❌ performans düşer
❌ uygulama kilitlenebilir

 ? Kodun yaptığı şey
public void Dispose()
{
    _context.Dispose();
    GC.SuppressFinalize(this);
}

1️⃣ _context.Dispose();

DbContext’i kapatır.
Bağlantılar serbest bırakılır.

En kritik satır budur ⭐

2️⃣ GC.SuppressFinalize(this);

Der ki: Bu nesne için Garbage Collector (çöp toplayıcısı) tekrar temizlik yapmaya uğraşmasın.
Çünkü biz zaten temizledik. Bu performans optimizasyonudur.

Neden UnitOfWork içinde? Çünkü: UnitOfWork = iş birimi Yani:
ürün ekle
stok düş
sipariş oluştur


işi bitince → hepsi commit edilir → context kapanır.

! Gerçek hayat benzetmesi
Restorana gittin.
Siparişleri verdin.
Yemeği yedin.
Hesabı ödedin.

Sonra:
👉 masada oturmaya devam eder misin?
! Hayır.
Kalkarsın.
Dispose = kalkmak 😄

ASP.NET Core’da kullanılıyor mu?
Evet ama çoğu zaman Dependency Injection otomatik yönetir.
Scope bitince:
👉 context dispose edilir.
Ama UnitOfWork yazıyorsan bu mimarinin parçasıdır.

! Dispose olmazsa ne olur?

Bir süre sonra:
⚠ Too many connections
⚠ Memory leak
⚠ Timeout
⚠ sistem yavaşlar

Kısaca ezber cümle 🎯

Dispose = kullanılan kaynağı güvenli şekilde kapatmak

Senior bilgisi ⭐
DbContext unmanaged resource tuttuğu için IDisposable’dır.
Bu yüzden zincirleme olarak UnitOfWork de IDisposable olur.

*/