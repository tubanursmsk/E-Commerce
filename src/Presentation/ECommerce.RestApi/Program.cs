using ECommerce.Application;
using ECommerce.Infrastructure;
using ECommerce.RestApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text;
using ECommerce.Application.Helpers;
using ECommerce.RestApi.Authorization;
using ECommerce.RestApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog Yapılandırması
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Konsola yaz
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Günlük dosya oluştur
    .CreateLogger();

builder.Host.UseSerilog(); // Default logger yerine Serilog kullan


// Controllers
builder.Services.AddControllers();

// CORS Policy --> RestApi'ye admin panel ve angular uygulamasından istek kabul etmek için

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                //"http://localhost:4000", // Angular
                "http://localhost:5176"  // MVC Admin
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // JWT + Cookie için gerekli
    });
});


// Swagger + JWT desteği
builder.Services.AddSwaggerWithJwt();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false, // Test aşamasında Audience hatasını devre dışı bırakıyoruz
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });


// Authorization Policy - Şirket Veri İzolasyonu
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CompanyIsolation", policy =>
        policy.Requirements.Add(new ECommerce.RestApi.Authorization.CompanyDataRequirement()));
});

// Handler'ı sisteme kaydediyoruz
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, ECommerce.RestApi.Authorization.CompanyDataIsolationHandler>();


// Application & Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration); // Infrastructure katmanındaki yazdığımız AddInfrastructureServices metodunu çağırıyoruz
builder.Services.AddApplicationServices();// Application katmanındaki tüm servisleri tek satırla ekliyoruz yani AddAutoMapper ile yaptığımız iş
builder.Services.AddHttpContextAccessor(); // Handler içinde HttpContext'e erişmek için şart.
builder.Services.AddOpenApi();


var app = builder.Build();

// Swagger UI Active
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Rest API v1");
        options.RoutePrefix = string.Empty; // http://localhost:5271/ adresinden erişim için
    });
}

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}*/

// Middleware
//app.UseHttpsRedirection();
app.UseMiddleware<ECommerce.RestApi.Middleware.GlobalExceptionHandler>();
app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

