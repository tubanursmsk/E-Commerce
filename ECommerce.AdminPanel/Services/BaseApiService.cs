using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECommerce.Application.Responses;

namespace ECommerce.AdminPanel.Services;
public class BaseApiService
{
    private readonly HttpClient _httpClient; 
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public BaseApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // CS8604 Çözümü: Null kontrolü ekliyoruz veya varsayılan değer veriyoruz
        var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5271/api/";
        _httpClient.BaseAddress = new Uri(baseUrl);

        var apiKey = _configuration["ApiSettings:ApiKey"] ?? string.Empty;
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
    }

    private void AddTokenToHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["JwtToken"];
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    // CS8603 Çözümü: Geri dönüş tipini '?' ile nullable yaparak veya null gelirse yeni nesne dönerek çözüyoruz
    /*public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint)
    {
        AddTokenToHeader();
        var response = await _httpClient.GetAsync(endpoint);
        var content = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
    }*/
    public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint)
{
    AddTokenToHeader();
    var response = await _httpClient.GetAsync(endpoint);
    
    // Eğer istek başarısızsa (401, 403, 500 vb.)
    if (!response.IsSuccessStatusCode)
    {
        return new ApiResponse<T> 
        { 
            Success = false, 
            Message = $"API Hatası: {response.StatusCode}" 
        };
    }

    var content = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
}


    public async Task<ApiResponse<TResponse>?> PostAsync<TRequest, TResponse>(string endpoint, TRequest dto)
    {
        AddTokenToHeader();
        var json = JsonSerializer.Serialize(dto);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, data);
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ApiResponse<TResponse>>(content, _jsonOptions);
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