# 🛠️ RestAPI & MVC Admin Panel Teknik Kontrol Listesi (Checklist)

 ## 🧠 Neyi, nasıl yaptık?
 
- **Mimari Yapı:** Clean Architecture prensiplerine göre katmanlı (Domain, Application, Infrastructure, Presentation) yapı kuruldu.
- **Veri Erişimi:** Generic Repository ve Unit of Work desenleri kullanılarak veritabanı bağımsızlığı ve işlem (Transaction) güvenliği sağlandı.
- **Güvenlik (Layer 1):** JWT (JSON Web Token) tabanlı yetkilendirme ve Role-Based Access Control (RBAC) uygulandı.
- **Güvenlik (Layer 2):** API Key koruması (Custom Attribute/Filter ile) eklenerek sadece izinli istemcilerin erişimi sağlandı.
- **Çoklu Kiracı (Multi-Tenancy):** Company Isolation (Şirket İzolasyonu) mantığıyla her şirketin sadece kendi verilerini görmesi/yönetmesi sağlandı.
- **Hata Yönetimi:** Global Exception Middleware ile hatalar merkezi bir noktada yakalanıp standart bir ApiResponse formatında döndürüldü.
- **Loglama:** Serilog entegrasyonu ile hatalar ve kritik işlemler hem konsola hem de günlük dosyalara (Rolling File) kaydedildi.
- **İlişkisel Veri:** AutoMapper ile Entity-DTO dönüşümleri sağlandı, Include (Eager Loading) ile ilişkili tablolar (Product-Category-Brand vb.) yönetildi.


---


> ### 1. "Neden Unit of Work ve Generic Repository kullandık?" 
- Veritabanı işlemlerini merkezi bir noktadan yönetmek ve kod tekrarını önlemek için Generic Repository kullandım. Unit of Work ise özellikle Sipariş (Order) oluşturma gibi birden fazla tablonun güncellendiği durumlarda, tüm işlemlerin tek bir 'transaction' içinde gerçekleşmesini sağlıyor. Eğer stok düşerken hata olursa, sipariş kaydı da yapılmıyor. Bu da veritabanı bütünlüğünü (Data Integrity) koruyor.


> ### 2. "API Güvenliğini nasıl sağladık?"
- İki aşamalı bir güvenlik kurguladım. İlk olarak X-Api-Key kontrolü ile istemciyi doğruluyorum. İkinci aşamada ise JWT kullanarak kullanıcı kimliğini ve rollerini kontrol ediyorum. Ayrıca yazdığım CompanyIsolationFilter sayesinde, bir Admin'in veya kullanıcının sadece kendi yetki alanındaki (Şirket ID'sine bağlı) verilere erişmesini garanti altına alarak veri sızıntısını önledim.


> ### 3. "Global Exception Handling kullanmamızın avantajı nedir?" 
- Controller'lar içinde try-catch blokları yazarak kod kirliliği yaratmak yerine, merkezi bir Middleware yazdım. Bu sayede uygulama genelinde fırlatılan her türlü hatayı yakalayıp, kullanıcıya teknik detay vermeden (security best practice) anlamlı bir hata mesajı dönüyorum. Aynı zamanda bu hataları Serilog ile arka planda loglayarak debug sürecini kolaylaştırıyorum.


> ### 4. "Neden MVC içinden doğrudan veritabanına bağlanmadık da API kullandık?"
- Sistemi Decoupled (Bağımsız) tasarladım. Yarın bir mobil uygulama yazmak istediğimizde veya sistemi mikroservislere bölmek istediğimizde API hazır. MVC burada sadece bir 'Client' (istemci) görevi görüyor. Bu sayede sunucu tarafı mantığı ile kullanıcı arayüzünü birbirinden tamamen ayırarak (Separation of Concerns) güvenliği ve ölçeklenebilirliği artırdım.


> ### 5. "API ve MVC projelerini neden aynı solution içinde ama ayrı projeler olarak tutmayı tercih ettik?"
- Sistem mimarisini Loose Coupling (Gevşek Bağlılık) prensibine göre tasarladım. Bu sayede sunucu (Server) tarafındaki iş mantığı ile kullanıcı arayüzü (UI) katmanını birbirinden ayırdım. API'miz şu an sadece MVC projesine değil, istenildiği an bir mobil uygulamaya veya React/Angular gibi farklı bir frontend projesine de hizmet verebilir durumdadır.


> ### 6. "MVC Admin Panelde Neden HttpClientFactory kullandık?"
- Doğrudan new HttpClient() kullanmak yerine HttpClientFactory kullandım. Çünkü new HttpClient() kullanıldığında 'Socket Exhaustion' (soket tükenmesi) sorunu yaşanabiliyor. Factory yapısı, arka plandaki HttpMessageHandler nesnelerini yöneterek performansı optimize eder ve kaynak yönetimini sağlar.


> ### 7. "Authentication (Kimlik Doğrulama) vs Authorization (Yetkilendirme) farkını nasıl yönettik?"
- API bize sadece bir string (JWT) dönüyor. Bu JWT'nin içinde zaten Role ve CompanyId saklı. Bizim yapmamız gereken, API'den gelen bu şifreli JWT'yi MVC tarafında "çözüp" içindeki bilgileri MVC'nin kendi Cookie'sine (Claims) yerleştirmek.
  
- Aynı zamanda **BaseApiService** arka planda çaktırmadan Cookie'den JWT'yi alır ve isteğin kafasına (Header) Bearer {Token} olarak yapıştırarak **otomatik yetkilendirme** sağladık. BaseApiService içindeki JSON serileştirme ile de hata yönetimi ve API Key ekleme gibi işleri her seferinde tekrar yazmayarak **kod tekrarınıda** önlemiş olduk.

<img width="683" height="369" alt="image" src="https://github.com/user-attachments/assets/a8106fb5-c1aa-4ff1-9fbe-45b3d8c77692" />


> ### 8. "Neden MVC içinde de servis var?" 

-> **MVC İçindeki Servislerin İş Akışı (Neden Servis Yazıyoruz?)**
Aslında en büyük kafa karışıklığı burada yaşanıyor: "Madem Infrastructure katmanında servislerim var, neden MVC içinde tekrar servis yazıyorum?"

- MVC projesindeki servisler, Infrastructure katmanındaki servislerle aynı işi yapmaz.
- Infrastructure Servisleri (API Tarafı): Veritabanına gider, SQL çalıştırır, veriyi ham halde getirir.
- MVC Servisleri (Web Tarafı): Veritabanını tanımaz. Tek işi HTTP isteği atmaktır. Yani BaseApiService'i kullanarak API'ye "Bana ürünleri ver" der, gelen JSON'u alır ve View'a gönderir.

**İş Akışı Şöyledir:**
- MVC Controller: "Bana ürün listesi lazım" der ve kendi içindeki IProductWebService'i çağırır.
- MVC WebService: BaseApiService aracılığıyla API'deki ProductController'a bir GET isteği gönderir.
- RestAPI: İstek API'ye ulaşır, oradaki ProductService (Infrastructure) veritabanından veriyi çeker ve JSON olarak döner.
- MVC WebService: Gelen JSON'u DTO'ya dönüştürür ve Controller'a paslar.


> ### 9. "MVC'de Neden Session ve Cookie'yi Aynı Anda Kullandık?"
- Cookie: Sayfa koruması için gereklidir ([Authorize]).
- Session: Layout'un sağ üst köşesinde "Kullanıcı Adı" veya "Şirket ID" gibi bilgileri hızlıca string olarak okuyabilmek için çok pratiktir.
- BaseApiService: API'ye giderken token'ı çerezden (JwtToken) okuyacak şekilde kurguladık.

 **Özetle Kullanıcı bilgisini neden hem Cookie hem Session'da tuttuk sorusuna daha net cevap vermek gerekirse:**
- "Kimlik doğrulama ve yetkilendirme (Auth) için tarayıcı bazlı Cookie (JWT) kullanıyorum. Ancak Layout gibi arayüz bileşenlerinde kullanıcı adını veya profil resmini her seferinde DB'den veya API'den çekip performansı düşürmemek için, sunucu taraflı bir cache mekanizması olan Session'ı tercih ettim. Böylece UI ihtiyaçlarını hızlıca karşılıyorum."

> ### 10. Neden hem Register hem RegisterWithCompany metodu var?"
- "Sistemde iki farklı kayıt akışı var. Standart bir müşteri (Customer) sadece email ve şifre ile kaydolurken; bir şirket yöneticisi (CompanyManager), şirket tüzel kişilik bilgilerini de vermek zorunda. Bu, Single Responsibility (Tek Sorumluluk) prensibi gereği farklı DTO'lar ve metotlar gerektirir. Ayrıca şirket kaydı bir Transaction (İşlem) gerektirir; şirket oluşmazsa kullanıcı da oluşmamalıdır."

> ### 11. "Veriyi veritabanından tamamen siliyor muyuz?"
- "Hayır, veritabanında Soft Delete (Mantıksal Silme) uyguluyorum. Entity yapımda bulunan IsDeleted bayrağını (flag) true çekiyorum. Bu sayede veri güvenliğini sağlıyor ve geçmişe dönük sipariş verilerinin tutarlılığını koruyorum. API katmanındaki Repository seviyesinde global bir filtre (HasQueryFilter) kullanarak, silinmiş ürünlerin listeleme sorgularına dahil edilmesini engelliyorum."

> ### 12. "Neden URL yerine Token kullandın?"
- "Şirket bazlı filtrelemede companyId parametresini URL'den (QueryString veya Route) almak yerine doğrudan kullanıcının JWT (Claim) bilgilerinden okumayı tercih ettim. Bu yaklaşım Insecure Direct Object Reference (IDOR) açıklarını önler. Kullanıcı URL'deki ID'yi değiştirse bile, sistem arka planda Token içindeki gerçek kimliğine baktığı için asla yetkisi olmayan veriyi göremez. Bu, sunucu taraflı güvenliğin (Server-Side Security) bir gereğidir."

> ### 13. "Neden Generic Repository dışına çıkıp özel metot yazmayı tercih ettik?"
- "Generic Repository temel CRUD işlemleri için yeterli olsa da, ilişkili tabloların (Include) ve özel iş kurallarının (Company bazlı filtreleme) gerektiği durumlarda mimariyi bozmadan Specific Repository (Örn: ProductRepository) kullandım. Böylece servis katmanını karmaşık LINQ sorgularından arındırıp iş mantığına odaklanmasını sağladım."

> ### 14. "Kategorilerdeki alt-üst ilişkisini veritabanında nasıl kurguladık?"
- "Self-referencing (kendi kendine referans veren) bir yapı kullandım. Category tablosunda ParentId adında bir kolon var ve bu kolon yine Category tablosunun Id kolonuna (Primary Key) işaret eden bir Foreign Key'dir. Bu sayede sonsuz derinlikte alt kategori oluşturabiliyoruz."

> ### 15. "Order-Detail sayfasının çalışma kurgusunu nasıl yaptık?" 
- "Müşteri detay sayfasında Micro-FrontEnd mantığıyla hareket ettim. Sayfayı yüklerken önce ana müşteri bilgilerini, ardından asenkron olarak o müşteriye ait siparişleri çektim. OrderService katmanında yaptığım şirket bazlı filtreleme sayesinde, bir mağaza yöneticisinin müşterinin başka bir mağazadan verdiği siparişleri görmesini engelleyerek Veri Gizliliği (Data Privacy) standartlarını korudum."
