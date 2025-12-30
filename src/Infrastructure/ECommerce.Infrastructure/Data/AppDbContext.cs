using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Tablolarımız (DbSet)
    public DbSet<Company> Companies { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Cargo> Cargoes { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Customer> Customers { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // İlişkileri ve kısıtlamaları burada özelleştirebilirsin (Fluent API)
        // Örneğin: UserRole tablosunda UserId ve RoleId birleşimi benzersiz olmalı.
        modelBuilder.Entity<UserRole>()
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        // Şirket silindiğinde ürünlerin de silinmesi (Cascade) gibi ayarlar...


         // Order ve Customer arasındaki ilişkiyi açıkça tanımlıyoruz
    modelBuilder.Entity<Order>()
        .HasOne(o => o.Customer)       // Her siparişin bir müşterisi vardır
        .WithMany(c => c.Orders)      // Her müşterinin birçok siparişi olabilir
        .HasForeignKey(o => o.CustomerId) // Yabancı anahtar (Foreign Key) budur
        .OnDelete(DeleteBehavior.Restrict); // İsteğe bağlı: Müşteri silinince siparişler kalsın mı?
}
    }


/*

OnModelCreating metodu, Entity Framework Core (EF Core) tarafında "Fluent API" olarak adlandırılan yapılandırma kısmıdır.

En basit haliyle işlevi şudur: Sınıf (Entity) tanımlarında (Product.cs, User.cs vb.) belirtemediğin veya daha detaylı kurgulamak istediğin veritabanı kurallarını burada belirlersin.

Peki, senin projen için gerekli mi? Kesinlikle evet. Nedenlerini ve ne işe yaradığını tane tane açıklayayım:

1. Neden Gereklidir?
C# sınıflarında yazdığın kodlar (örneğin public Guid CompanyId { get; set; }), EF Core'a bazı temel bilgileri verir. Ancak bazı kurallar vardır ki bunları sadece sınıfa bakarak anlayamaz:

Çoklu Benzersizlik (Composite Index): Bir kullanıcının aynı role iki kez atanmasını engellemek istersen (UserRole tablosu), bunu sadece sınıfta belirtemezsin. OnModelCreating içinde "UserId ve RoleId ikilisi benzersiz olsun" demen gerekir.

Silme Davranışları (Cascade Delete): Bir Şirket (Company) silindiğinde, o şirkete ait tüm ürünler (Product) de silinsin mi? Yoksa hata mı versin? Bunu burada yönetirsin.

Hassas Veri Tipleri: Bir decimal alanın veritabanında decimal(18,2) (18 basamak, virgülden sonra 2 basamak) formatında saklanacağını burada garanti altına alırsın.

2. Kodundaki Örneklerin İşlevi
Senin paylaştığın kod üzerinden gidelim:

C#

modelBuilder.Entity<UserRole>()
    .HasIndex(ur => new { ur.UserId, ur.RoleId })
    .IsUnique();
İşlevi: Veritabanında UserRoles tablosuna bir "Index" (indeks) ekler.

Senaryo: Eğer bu kod olmazsa, bir kullanıcıya "Admin" rolünü yanlışlıkla 10 kere ekleyebilirsin. Bu kod sayesinde veritabanı, aynı kullanıcı-rol çiftinin ikinci kez eklenmesine izin vermez, hata fırlatır. Veri tutarlılığını korur.

3. Senin Projen İçin Eklememiz Gereken Önemli Kurallar
Yönergeye uygun olarak, bu kısmın içine şu tip kuralları da ileride ekleyeceğiz:

Fiyat Belirleme: Product içindeki Price alanı için:

C#

modelBuilder.Entity<Product>()
    .Property(p => p.Price)
    .HasPrecision(18, 2); // Kuruş hesaplamaları için hassasiyet
Şirket Bazlı İlişkiler: Şirket (Company) silindiğinde bağlı olduğu ürünlerin veya siparişlerin durumunu kontrol etmek için.

4. Alternatif: Data Annotations
Bazı kuralları sınıfların başına [Required], [MaxLength(100)] gibi nitelikler ekleyerek de yapabilirsin. Ancak profesyonel ve Clean Architecture yapısındaki projelerde (arkadaşının projesinde de böyledir), entity'lerin (nesnelerin) "temiz" kalması istenir. Bu yüzden veritabanı ayarlarının tamamı merkezi bir yerden, yani OnModelCreating içinden yönetilir.

Özetle: Bu kısım veritabanının "Anayasası" gibidir. Kuralları burada koyarsın ki veritabanın karmaşıklaşmasın ve hatalı veri kabul etmesin.
*/