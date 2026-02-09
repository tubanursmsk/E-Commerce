# 🛒 E-Commerce Platform (Multi-Tenant) — Admin Panel + REST API + Angular Storefront

> Bu proje; farklı şirketlerin kendi e-ticaret operasyonlarını yönetebildiği **çok katmanlı bir E-Ticaret Servis Platformu**dur.  
> Mimari; **REST API (çekirdek köprü)** üzerine kuruludur. Hem **Admin Panel** (şirket yönetimi) hem de **Angular Storefront** tüm veriye **API üzerinden** erişir. 

---

## 🎯 Proje Amacı

- Kurumsal düzeyde **Admin Panel (Company Management)** geliştirmek  
- Modern standartlara uygun **REST API** üretmek (JWT, Role Based, Swagger, DTO, AutoMapper, Validation, Logging) 
- **Angular** ile kullanıcıya yönelik **storefront** geliştirmek (auth guard, interceptor, lazy loading, search, pagination, responsive) 
- **Clean / Layered Architecture** yaklaşımını tek projede uçtan uca deneyimlemek

---

## 🧩 Genel Bakış

Bu repo 3 ana parçadan oluşur:

1. **REST API** → Sistemin çekirdeği / veri köprüsü  
2. **Admin Panel** → Şirketlerin ürün, kategori, marka, sipariş, müşteri vb. yönetimi  
3. **Client Application (Angular)** → Müşterinin alışveriş yaptığı storefront

📌 **Tek veri kapısı REST API’dir.** Admin Panel ve Angular Client doğrudan veritabanına bağlanmaz; yalnızca API tüketir. 

---

## 🧠 Mimari Yaklaşım

Bu proje, REST API’yi merkez alan, istemcilerin (Admin Panel + Angular Client) API üzerinden sisteme eriştiği çok katmanlı (Clean / Layered) mimari ile tasarlanmıştır.

### 1) Solution Yapısı

Proje **katmanlı mimari (layered architecture)** prensiplerine uygun olarak geliştirilmiştir.
```
E-Commerce/
├── ECommerce_RestApi/                       → REST API (Sistemin çekirdeği / köprü)
│   └── src/
│       ├── Core/
│       │   ├── ECommerce.Application/      → UseCase’ler, DTO, Interfaces, Helpers, Mappings, JWT
│       │   └── ECommerce.Domain/           → Entity’ler, Domain Interfaces, kurallar
│       ├── Infrastructure/                → DbContext, Migration, Repository, Service Implementations
│       └── Presentation/                  → Controllers, Filters, Middlewares, Attributes, Swagger, Config
│
├── ECommerce_AdminPanel/  → Admin UI (ASP.NET Core / Razor Pages veya MVC UI)
│   ├── Pages/Views/Controllers   → UI katmanı
│   ├── Services                  → API tüketen servisler / UI business
│   └── ...                       → Layout, static, helpers
│
└── ECommerce_ClientApplication/  → Client UI (Angular)
    └── src/
        ├── app/
        │   ├── core/             → auth, guards, interceptors, api services
        │   ├── features/         → ürünler, sepet, sipariş vb.
        │   ├── shared/           → ortak component/pipes
        │   └── layout/           → header/footer/shell
        └── environments/         → api base url vb.

```

## 2) Katmanların Sorumlulukları
### ✅ Domain (Core/Domain)

- Sistemin iş kuralları, entity modelleri
- Bağımlılık almaz, “en saf” katmandır

### ✅ Application (Core/Application)

- Use-case odaklı iş akışları
- DTO’lar, interface sözleşmeleri, mapping profilleri
- Auth/JWT, helper yapıları (gerekli olanlar)

“Ne yapılacak?” burada tanımlanır

### ✅ Infrastructure

“Nasıl yapılacak?” kısmı

- EF Core DbContext, Migration’lar, Repository implementasyonları
- Harici servis / veri erişim implementasyonları

### ✅ Presentation (REST API)

- Controller’lar ile HTTP endpointleri
- Authorization/Filters/Middlewares
- Swagger ve API konfigürasyonları

İstemcilerin tek giriş kapısı

##  3) İstemci Uygulamalar
### ✅ Admin Panel (ASP.NET Core UI)

- Admin / CompanyManager / Staff gibi rollerin yönetim ekranı
- Ürün/Kategori/Marka/Sipariş vb. CRUD işlemleri
- Veriye erişmek için doğrudan DB’ye değil API’ye gider
- Role-based ekran ve aksiyon yönetimi

### ✅ Client Application (Angular)

- Müşterinin alışveriş yaptığı arayüz
- Ürün listeleme, detay, sepet, sipariş süreçleri
- Tüm işlemler için REST API üzerinden iletişim kurar

##  4) Temel Prensip

📌 Tek veri kapısı REST API’dir.
AdminPanel ve Angular Client DB’ye doğrudan erişmez, sadece API tüketir.
Bu yaklaşım; güvenlik, ölçeklenebilirlik ve bakım kolaylığı sağlar.

---

📌 **UI → Service → DbContext** zinciri korunur  
📌 Razor Pages doğrudan DbContext’e erişmez  
📌 Tüm işlemler kullanıcı bazlı filtrelenir

---

## 🔐 Kimlik Doğrulama & Güvenlik (Admin Panel Tarafı)

- Cookie tabanlı authentication
- `ClaimTypes.NameIdentifier` ile kullanıcı tanımlama
- Şirket sahibi veya personel yalnızca **kendi markalarını, kategorilerini ve ürünlerini** görür
- Multi-tenant veri izolasyonu

---

# 🧩 Modüller & Özellikler

Aşağıdaki modüller hem Admin Panel hem de REST API tarafında (ilgili rollere göre) kurgulanmıştır: 

### Admin Panel (Şirket Yönetimi)

- Dashboard (toplam ürün/kullanıcı/sipariş/yorum)
- Ürün Yönetimi (listeleme, ekleme, güncelleme, silme, marka/kategori bağlama)
- Kategori Yönetimi (CRUD)
- Marka Yönetimi (CRUD)
- Sipariş Yönetimi (durum güncelleme, iptal/iade/kargo)
- Müşteri Yönetimi
- Yorum Yönetimi
- Kargo Ayarları
- Genel Ayarlar
- Kullanıcı & Rol Yönetimi
- Banner/Slider, ürün görsel yönetimi

### REST API (Sistemin çekirdeği)
- Products (Pagination + Search)
- Categories, Brands
- Customers
- Orders (status update, refund, cancel, tracking)
- Reviews
- Banners
- Admin Users & Roles
- Standart response formatı:
  
```json
{ "success": true, "message": "", "data": {} 
```

### Angular Storefront

- Anasayfa + slider
- Kategoriye göre ürün listeleme
- Ürün detay
- Sepet yönetimi
- Üye kayıt & giriş (JWT)
- Profil & adres yönetimi
- Checkout / sipariş oluşturma
- Sipariş geçmişi
- Yorum ekleme
- 404 / 500 hata sayfaları
- Global exception interceptor, loading spinner, pagination, search, responsive zorunlulukları 

## 🔐 Güvenlik

### REST API
- JWT Authentication
- Role Based Authorization
- Şirket bazlı API Key desteği 
- Global Exception Handling Middleware + Logging (Serilog vb.) + Swagger zorunlu 

### Admin Panel
- Session / Cookie tabanlı oturum yönetimi
- Sayfa/rol bazlı yetkilendirme
- Validation + Custom Error Handling zorunlu 


### Angular
- JWT login
- Auth Guard + Role Guard
- Auth Interceptor (token ekleme)
- Global Exception Interceptor 

---

## ⚙️ Kullanılan Teknolojiler

| Katman | Teknoloji |
|------|-----------|
| REST API | ASP.NET Core Web API, EF Core, AutoMapper, Swagger, JWT, Middleware |
| Admin Panel | ASP.NET Core MVC, Bootstrap 5 |
| FrontEnd | Angular, Guards, Interceptors, Lazy Loading |
| Veritabanı | SQLite |
| Auth | Cookie Authentication |
| Session | ASP.NET Session |

---

## 🚀 Kurulum & Çalıştırma

### 🧩 REST API — Kurulum
```bash
# Repo'yu klonla
git clone https://github.com/tubanursmsk/E-Commerce.git
cd E-Commerce

# Bağımlılıkları yükle
dotnet restore

# Veritabanını oluştur
dotnet ef database update

# Uygulamayı çalıştır
dotnet run
```

```arduino
http://localhost:5271
```

### 🧩 Admin Panel — Kurulum
```bash
cd AdminPanel
dotnet restore
dotnet run
```
```arduino
http://localhost:5176
```
- Admin Panel, veri işlemleri için REST API’ye istek atar (tasarım/kurgu bu şekildedir).

### 🧩 Angular Client — Kurulum
```bash
cd FrontEnd
npm install
ng serve
```
```arduino
http://localhost:4200
```

## 🗄️ Veritabanı

- SQLite kullanılmıştır
- EF Core migrations desteklidir
- Her tablo UserId ile filtrelenir (multi-tenant yapı)

---

## 👥 Demo Hesaplar

| Rol             | Email                  | Şifre   | 
|-----------------|------------------------|---------|
| Admin           | ali@mail.com           | Aa12345 |
| CompanyManager  | hasan@mail.com         | Aa12345 |
| Staff           | veli@mail.com          | Aa12345 |
| Customer        | metehanpolat@mail.com  | Aa12345 |

---

## 📌 API Endpointleri (Özet)

- Aşağıdaki tablo proje yönergesindeki zorunlu modülleri temsil eder. Kesin endpoint rotaları için Swagger esas alınır.
  
| Modül               | Açıklama                                   |
| ------------------- | ------------------------------------------ |
| Auth                | JWT Login/Register/Profile                 |
| Products            | Pagination + Search + CRUD                 |
| Categories          | CRUD                                       |
| Brands              | CRUD                                       |
| Customers           | CRUD                                       |
| Orders              | Status update / refund / cancel / tracking |
| Reviews             | CRUD                                       |
| Banners             | CRUD                                       |
| Admin Users & Roles | RBAC yönetimi                              |

---

## 🔄 İş Akışı (Flow Diagram)

### Sistem Seviyesi Akış (AdminPanel + Client → API → DB)
```mermaid
graph LR
  A[Admin Panel] -->|HTTP| B[REST API]
  C[Angular Client] -->|HTTP| B[REST API]
  B --> D[(Database)]
```

### Admin Panel Akışı (Marka / Kategori / Ürün)
```mermaid
graph TB
    A[👤 Kullanıcı Girişi] --> B{🔐 Kimlik Doğrulama}
    B -->|✅ Başarılı| C[📊 Dashboard]
    B -->|❌ Başarısız| D[🔒 Login Sayfası]

    C --> E[🏷️ Marka Yönetimi]
    C --> F[🗂️ Kategori Yönetimi]
    C --> G[📦 Ürün Yönetimi]
    C --> H[🧾 Sipariş / 👥 Müşteri / 💬 Yorum vb.]

    E --> H[➕ Ekle / ✏️ Güncelle / 🗑️ Sil]
    F --> I[➕ Ekle / ✏️ Güncelle / 🗑️ Sil]
    G --> G1[➕ Ürün Ekle / 🖼️ Görsel / 🔎 Search / 📄 Pagination]

    E1 --> DB[(🗄️ DB)]
    F1 --> DB
    G1 --> DB
```

---


## 🖼️ Ekran Görüntüleri / Videolar

### Swagger - Rest API Dokümantasyonu
<img width="683" height="369" alt="image" src="https://github.com/user-attachments/assets/e49b5ad0-ee0c-4bad-8d3f-b1cc82758b77" />

<img width="683" height="369" alt="image" src="https://github.com/user-attachments/assets/c054ec8a-0641-4839-a5a1-62661741d20f" />

<img width="683" height="369" alt="image" src="https://github.com/user-attachments/assets/6ae323f0-e455-4887-86ba-69a6fe414802" />


---


## 👩‍💻 Admin Panel 

> ### Dashboard (toplam ürün/kullanıcı/sipariş/yorum)

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/3f4ce33f-ad59-43be-aef9-40b476bb0d19" />
  
> ### Ürün Yönetimi (listeleme, ekleme, güncelleme, silme, marka/kategori bağlama)

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/008e2a32-86a2-41de-994b-d037b3f89f17" />
  
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/9db492c5-f785-4bbe-bbf6-601ce8cacc8c" />

- Patron veya muhasebeci "Stok listesini bana at" dediğinde veritabanına gitmeye gerek kalmadan, tek tuşla (.xsln formatında) profesyonel bir rapor sunma imkanı! 🚀
  
<img width="683" height="369" alt="image" src="https://github.com/user-attachments/assets/35f0562c-79c2-40a0-8e09-274dcf798513" />

<img width="683" height="369" alt="image" src="https://github.com/user-attachments/assets/77bd549e-c98e-4bd3-9eb6-6f35092d0350" />

> ### Kategori Yönetimi (CRUD)

<img width="683" height="369" alt="image" src="https://github.com/user-attachments/assets/66a9e100-e0a9-450b-af08-aec6fed4f772" />

> ### Marka Yönetimi (CRUD)

<img width="683" height="369" alt="image" src="https://github.com/user-attachments/assets/90870260-91c2-4cf9-b4b0-90f1e61c18fb" />

> ### Sipariş Yönetimi (durum güncelleme, iptal/iade/kargo)

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/069d3a18-7778-4908-81d1-18d62e2331f5" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/ff46da99-23c1-4cbc-8acd-b4e7415bc59b" />


> ### Müşteri Yönetimi

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/a4a46262-9790-4169-89ff-48b9d2d7312b" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/047e2661-dbe7-4105-bb44-821d50c47086" />

> ### Yorum Yönetimi

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/26730f68-c0fc-4507-b548-0144627b4423" />

> ### Kargo Ayarları

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/6a3d21a5-0f78-41d0-a964-a556507084aa" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/ae4ec7a7-4472-49e5-b4a1-f62834a203cf" />

> ### Kullanıcı & Rol Yönetimi

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/35ff2b94-6810-4477-bee8-95ff0aa0ef8e" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/4c9cb668-c208-4211-8742-022f96b2df00" />


> ### Banner/Slider, ürün görsel yönetimi

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/c2464c2c-3fd6-4353-bdfa-fed9c83064cd" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/8f8f7661-02d6-478e-8d3b-9e5f6ccc3fcf" />

> ### Şirket Ayarları

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/8c2e5275-27d3-42fa-9e2f-a84c2590b7a5" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/398ef1fc-5737-4ef7-a10f-0c2207defdc7" />

> ### Profil Ayarları

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/a01908f3-a1dd-4abc-9c33-c763bcbbc6e0" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/7f9b58d0-ebaf-46bc-840c-099b05f8811c" />

> ### Müşteri kayıt & giriş (JWT)

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/b2e7fb97-6327-42df-854f-8dc4a3b7089d" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/f74b88af-60ca-499d-9512-272bb39c0225" />


---


## 🧩 Angular Client

> ### Anasayfa + slider
[Macbook-Air-localhost-yy_8cjr47hc7_n.webm](https://github.com/user-attachments/assets/2eae17c9-14fa-4139-81ed-beb4aa70dc83)

[Macbook-Air-localhost-5943v4ms2e1a5y.webm](https://github.com/user-attachments/assets/b407c5de-fa0c-4fe6-a92f-1fa5df5e5f4f)

> ### Kategoriye göre ürün listeleme

[Macbook-Air-localhost-tdr7wo63q2hy6z.webm](https://github.com/user-attachments/assets/6f2cfcfd-e898-44e3-850d-2f6f1e17991c)

> ### Ürün detay

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/118292a8-abc4-4ae7-9176-ab686dc597a7" />

[Macbook-Air-localhost-a0mcx42v2-k0s9.webm](https://github.com/user-attachments/assets/fbe286b6-1eb3-478d-8d63-75685dbbb274)

> ### Sepet yönetimi

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/9dafbda1-a708-48a5-8693-95e61619fedc" />

> ### Üye kayıt & giriş (JWT)

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/81beb9c8-9f49-4645-a15f-95ca707ebbb8" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/42edd46d-637e-45c7-85bb-ffbe7d1b975c" />

> ### Profil & adres yönetimi

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/3876743e-149c-4316-a177-088fa94d619e" />

> ### Checkout / sipariş oluşturma

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/a16b373f-bc9d-49ca-85aa-8a35ad9153e1" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/4f65b61f-1df4-4f0e-a81c-5206cc82bdfd" />


> ### Sipariş geçmişi

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/50c3aeb9-4864-42b3-8353-5b587c658302" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/8462ab89-c3d0-42b1-a598-8ac1b1fcc7a5" />


> ### Yorum ekleme

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/11031c4f-c494-45f1-ad10-dbe667e0dbb6" />


> ### 404 / 500 hata sayfaları

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/71a71d32-b258-4432-916e-1061d99156b1" />

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/07652906-ad11-4ea9-8bd6-a7ada7610166" />


---


> ## 🎓 Öğrenme Kazanımları
- Katmanlı mimariyi (Core/Application/Domain + Infrastructure + Presentation) gerçek proje üstünde uygulama
- Admin Panel + API + Angular istemci entegrasyonu
- JWT / Session / Role Based Authorization pratikleri
- Global exception handling + logging + validation standartları

---

## 👩‍💻 Geliştirici
**Tuba Nur Şimşek** (Software Developer)

```bash
🔗 GitHub: https://github.com/tubanursmsk
```

---

## 🧾 Lisans

MIT License © 2025 — tubanursmsk

---

> ## 🏷️ Etiketler
`.NET Razor Pages Entity Framework Core SQLite`
`Admin Panel E-Commerce Multi-Tenant CRUD`
`Layered Architecture Bootstrap Backend Development` `C#`
`Console App` `Katmanlı Mimari` `ASP.Net Core`

