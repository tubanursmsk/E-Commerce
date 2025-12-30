using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECommerce.Application.Responses;

namespace ECommerce.AdminPanel.Services;

public class BaseApiService
{
    private readonly HttpClient _httpClient; // HttpClient, başka bir sunucuya HTTP isteği atmanı sağlar. MVC → API çağrısı
    private readonly IConfiguration _configuration; // IConfiguration, appsettings.json gibi yapılandırma dosyalarına erişmeni sağlar.
    private readonly IHttpContextAccessor _httpContextAccessor; // Service normalde HttpContext’i görmez. Bunu ekleyerek mvc içindeki Cookie’lere erişebiliyoruz.
    private readonly JsonSerializerOptions _jsonOptions;
    public BaseApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Null kontrolü ekliyoruz veya varsayılan değer veriyoruz
        var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5271/api/"; //Tarayıcıda gördüğün http://localhost:5271/index.html Swagger arayüzüdür. BaseApiService ise bu arayüzü değil, arkadaki veriyi (endpointleri) çağırır. Yani ikisi aynı kök adresi (localhost:5271) kullanır, sadece yolları farklıdır.
        _httpClient.BaseAddress = new Uri(baseUrl); //URI (Uniform Resource Identifier) Basitçe: Bir internet kaynağının adresini temsil eden nesne. yani API'nin temel adresi. uri ve ulr aynlamda değildir. Bu satır şunu yapar: “Bu HttpClient, varsayılan olarak bu API’ye konuşacak.”

        var apiKey = _configuration["ApiSettings:ApiKey"] ?? string.Empty;
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
    }
   private void AddTokenToHeader()
{
    // Önce mevcut Authorization header'ı temizle
    _httpClient.DefaultRequestHeaders.Remove("Authorization");
    
    var token = _httpContextAccessor.HttpContext?.Request.Cookies["JwtToken"];
    
    if (!string.IsNullOrEmpty(token))
    {
        Console.WriteLine($"Token found: {token.Substring(0, Math.Min(20, token.Length))}...");
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }
    else
    {
        Console.WriteLine("JWT Token not found in cookies!");
    }
}
    // Geri dönüş tipini '?' ile nullable yaparak veya null gelirse yeni nesne dönerek çözüyoruz
    public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint)
    {
        try
        {
            AddTokenToHeader();
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Hata durumunu kontrol et
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Error: {response.StatusCode}");
                Console.WriteLine($"Response: {content}");

                // 403 hatasında HTML dönüyor olabilir, JSON parse etmeye çalışma
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = "Erişim engellendi. Token geçersiz veya yetkiniz yok.",
                        Data = default
                    };
                }

                // Diğer hatalar
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = $"API Hatası: {response.StatusCode} - {content}",
                    Data = default
                };
            }

            return JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAsync Exception: {ex.Message}");
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"İstek sırasında hata: {ex.Message}",
                Data = default
            };
        }
    }
    public async Task<ApiResponse<TResponse>?> PostAsync<TRequest, TResponse>(string endpoint, TRequest dto)
    {
        try
        {
            AddTokenToHeader();
            var json = JsonSerializer.Serialize(dto);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, data);
            var content = await response.Content.ReadAsStringAsync();

            // Hata durumunu kontrol et
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Post Error: {response.StatusCode}");
                Console.WriteLine($"Response: {content}");

                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = $"API Hatası: {response.StatusCode} - {content}",
                    Data = default
                };
            }

            return JsonSerializer.Deserialize<ApiResponse<TResponse>>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PostAsync Exception: {ex.Message}");
            return new ApiResponse<TResponse>
            {
                Success = false,
                Message = $"İstek sırasında hata: {ex.Message}",
                Data = default
            };
        }
    }
}

/*

notlar: 
1. HttpClient Nedir?
   - HttpClient, başka bir sunucuya HTTP isteği atmanı sağlar. MVC → API çağrısı gibi.

Bu BaseApiService sınıfı neden “beyin”?
MVC tarafında hiçbir controller:
HttpClient ayarı yapmaz
Token eklemez
ApiKey düşünmez
Hepsi buradan geçer.

Kısaca BaseApiService = MVC’nin API ile konuşan tercümanı

*/