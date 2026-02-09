# 🛠️ RestAPI Teknik Kontrol Listesi (Checklist)

 "Neyi, nasıl yaptık?"

Mimari Yapı: Clean Architecture prensiplerine göre katmanlı (Domain, Application, Infrastructure, Presentation) yapı kuruldu.

Veri Erişimi: Generic Repository ve Unit of Work desenleri kullanılarak veritabanı bağımsızlığı ve işlem (Transaction) güvenliği sağlandı.

Güvenlik (Layer 1): JWT (JSON Web Token) tabanlı yetkilendirme ve Role-Based Access Control (RBAC) uygulandı.

Güvenlik (Layer 2): API Key koruması (Custom Attribute/Filter ile) eklenerek sadece izinli istemcilerin erişimi sağlandı.

Çoklu Kiracı (Multi-Tenancy): Company Isolation (Şirket İzolasyonu) mantığıyla her şirketin sadece kendi verilerini görmesi/yönetmesi sağlandı.

Hata Yönetimi: Global Exception Middleware ile hatalar merkezi bir noktada yakalanıp standart bir ApiResponse formatında döndürüldü.

Loglama: Serilog entegrasyonu ile hatalar ve kritik işlemler hem konsola hem de günlük dosyalara (Rolling File) kaydedildi.

İlişkisel Veri: AutoMapper ile Entity-DTO dönüşümleri sağlandı, Include (Eager Loading) ile ilişkili tablolar (Product-Category-Brand vb.) yönetildi.


1. "Neden Unit of Work ve Generic Repository kullandın?"
Cevap: "Veritabanı işlemlerini merkezi bir noktadan yönetmek ve kod tekrarını önlemek için Generic Repository kullandım. Unit of Work ise özellikle Sipariş (Order) oluşturma gibi birden fazla tablonun güncellendiği durumlarda, tüm işlemlerin tek bir 'transaction' içinde gerçekleşmesini sağlıyor. Eğer stok düşerken hata olursa, sipariş kaydı da yapılmıyor. Bu da veritabanı bütünlüğünü (Data Integrity) koruyor."

2. "API Güvenliğini nasıl sağladın?"
Cevap: "İki aşamalı bir güvenlik kurguladım. İlk olarak X-Api-Key kontrolü ile istemciyi doğruluyorum. İkinci aşamada ise JWT kullanarak kullanıcı kimliğini ve rollerini kontrol ediyorum. Ayrıca yazdığım CompanyIsolationFilter sayesinde, bir Admin'in veya kullanıcının sadece kendi yetki alanındaki (Şirket ID'sine bağlı) verilere erişmesini garanti altına alarak veri sızıntısını önledim."

3. "Global Exception Handling kullanmanın avantajı nedir?"
Cevap: "Controller'lar içinde try-catch blokları yazarak kod kirliliği yaratmak yerine, merkezi bir Middleware yazdım. Bu sayede uygulama genelinde fırlatılan her türlü hatayı yakalayıp, kullanıcıya teknik detay vermeden (security best practice) anlamlı bir hata mesajı dönüyorum. Aynı zamanda bu hataları Serilog ile arka planda loglayarak debug sürecini kolaylaştırıyorum."
