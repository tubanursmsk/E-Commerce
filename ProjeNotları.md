# 🛠️ RestAPI Teknik Kontrol Listesi (Checklist)

 ## 🧠 Neyi, nasıl yaptık?
 
- **Mimari Yapı:** Clean Architecture prensiplerine göre katmanlı (Domain, Application, Infrastructure, Presentation) yapı kuruldu.
- **Veri Erişimi:** Generic Repository ve Unit of Work desenleri kullanılarak veritabanı bağımsızlığı ve işlem (Transaction) güvenliği sağlandı.
- **Güvenlik (Layer 1):** JWT (JSON Web Token) tabanlı yetkilendirme ve Role-Based Access Control (RBAC) uygulandı.
- **Güvenlik (Layer 2):** API Key koruması (Custom Attribute/Filter ile) eklenerek sadece izinli istemcilerin erişimi sağlandı.
- **Çoklu Kiracı (Multi-Tenancy):** Company Isolation (Şirket İzolasyonu) mantığıyla her şirketin sadece kendi verilerini görmesi/yönetmesi sağlandı.
- **Hata Yönetimi:** Global Exception Middleware ile hatalar merkezi bir noktada yakalanıp standart bir ApiResponse formatında döndürüldü.
- **Loglama:** Serilog entegrasyonu ile hatalar ve kritik işlemler hem konsola hem de günlük dosyalara (Rolling File) kaydedildi.
- **İlişkisel Veri:** AutoMapper ile Entity-DTO dönüşümleri sağlandı, Include (Eager Loading) ile ilişkili tablolar (Product-Category-Brand vb.) yönetildi.



> ### 1. "Neden Unit of Work ve Generic Repository kullandık?" 
- Veritabanı işlemlerini merkezi bir noktadan yönetmek ve kod tekrarını önlemek için Generic Repository kullandım. Unit of Work ise özellikle Sipariş (Order) oluşturma gibi birden fazla tablonun güncellendiği durumlarda, tüm işlemlerin tek bir 'transaction' içinde gerçekleşmesini sağlıyor. Eğer stok düşerken hata olursa, sipariş kaydı da yapılmıyor. Bu da veritabanı bütünlüğünü (Data Integrity) koruyor.


### 2. "API Güvenliğini nasıl sağladık?"
- İki aşamalı bir güvenlik kurguladım. İlk olarak X-Api-Key kontrolü ile istemciyi doğruluyorum. İkinci aşamada ise JWT kullanarak kullanıcı kimliğini ve rollerini kontrol ediyorum. Ayrıca yazdığım CompanyIsolationFilter sayesinde, bir Admin'in veya kullanıcının sadece kendi yetki alanındaki (Şirket ID'sine bağlı) verilere erişmesini garanti altına alarak veri sızıntısını önledim.


### 3. "Global Exception Handling kullanmamızın avantajı nedir?" 
- Controller'lar içinde try-catch blokları yazarak kod kirliliği yaratmak yerine, merkezi bir Middleware yazdım. Bu sayede uygulama genelinde fırlatılan her türlü hatayı yakalayıp, kullanıcıya teknik detay vermeden (security best practice) anlamlı bir hata mesajı dönüyorum. Aynı zamanda bu hataları Serilog ile arka planda loglayarak debug sürecini kolaylaştırıyorum.


### 4. "Neden MVC içinden doğrudan veritabanına bağlanmadık da API kullandık?"
- Sistemi Decoupled (Bağımsız) tasarladım. Yarın bir mobil uygulama yazmak istediğimizde veya sistemi mikroservislere bölmek istediğimizde API hazır. MVC burada sadece bir 'Client' (istemci) görevi görüyor. Bu sayede sunucu tarafı mantığı ile kullanıcı arayüzünü birbirinden tamamen ayırarak (Separation of Concerns) güvenliği ve ölçeklenebilirliği artırdım.


### 5. "API ve MVC projelerini neden aynı solution içinde ama ayrı projeler olarak tutmayı tercih ettik?"
- Sistem mimarisini Loose Coupling (Gevşek Bağlılık) prensibine göre tasarladım. Bu sayede sunucu (Server) tarafındaki iş mantığı ile kullanıcı arayüzü (UI) katmanını birbirinden ayırdım. API'miz şu an sadece MVC projesine değil, istenildiği an bir mobil uygulamaya veya React/Angular gibi farklı bir frontend projesine de hizmet verebilir durumdadır.


### 6. "MVC Admin Panelde Neden HttpClientFactory kullandık?"
- Doğrudan new HttpClient() kullanmak yerine HttpClientFactory kullandım. Çünkü new HttpClient() kullanıldığında 'Socket Exhaustion' (soket tükenmesi) sorunu yaşanabiliyor. Factory yapısı, arka plandaki HttpMessageHandler nesnelerini yöneterek performansı optimize eder ve kaynak yönetimini sağlar.


### 7. "Authentication (Kimlik Doğrulama) vs Authorization (Yetkilendirme) farkını nasıl yönettik?"
- API bize sadece bir string (JWT) dönüyor. Bu JWT'nin içinde zaten Role ve CompanyId saklı. Bizim yapmamız gereken, API'den gelen bu şifreli JWT'yi MVC tarafında "çözüp" içindeki bilgileri MVC'nin kendi Cookie'sine (Claims) yerleştirmek.
- Aynı zamanda **BaseApiService** arka planda çaktırmadan Cookie'den JWT'yi alır ve isteğin kafasına (Header) Bearer {Token} olarak yapıştırarak **otomatik yetkilendirme** sağladık. BaseApiService içindeki JSON serileştirme ile de hata yönetimi ve API Key ekleme gibi işleri her seferinde tekrar yazmayarak **kod tekrarınıda** önlemiş olduk.

<img width="683" height="369" alt="image" src="https://github.com/user-attachments/assets/a8106fb5-c1aa-4ff1-9fbe-45b3d8c77692" />


### 8. 

