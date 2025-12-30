using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IUnitOfWork 
{
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    ICustomerRepository Customers { get; }
    ICompanyRepository Companies { get; }
    
    // Özel repository gerektirmeyenler için Generic Repository kullanabiliriz
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<Brand> Brands { get; }
    IGenericRepository<Review> Reviews { get; }
    IGenericRepository<Banner> Banners { get; }
    IGenericRepository<User> Users { get; }
    IGenericRepository<Role> Roles { get; }
    IGenericRepository<Request> Requests { get; }
    IGenericRepository<Cargo> Cargoes { get; }
  

    Task<int> SaveChangesAsync();
}

/*1. IUnitOfWork (Plan / Sözleşme)
Nerede: Core.Domain katmanında. Görevi: Diğer katmanlara (örneğin Application içindeki ProductService'e) şu 
mesajı verir: "Benimle çalışmak istiyorsan, sana ürünleri, siparişleri ve kaydetme butonunu (SaveChangesAsync)
 sunacağımı garanti ediyorum. Ama bu işin veritabanı tarafında (SQL mi, Oracle mı) nasıl yapıldığı seni 
 ilgilendirmez."

Neden gerekli? Eğer sen yarın veritabanını SQLite'tan SQL Server'a taşırsan, Application katmanındaki
 hiçbir kodu değiştirmezsin. Çünkü o katman sadece "Plan"a (IUnitOfWork) bakar. */