using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;
using ECommerce.RestApi.Filters; // [ApiKey] attribute'una erişmek için

namespace ECommerce.Application.Helpers
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "🚀 Rest API Dokümantasyonu",
                    Version = "v1.0.0",
                    Description = @"**JWT + Role + API Key tabanlı doğrulama** destekli REST API.",
                    // ... diğer contact/license bilgilerin kalsın
                });

                // ✅ 1. JWT Tanımı
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT token girin. Örn: **Bearer {token}**"
                });

                // ✅ 2. X-Api-Key Tanımı (YENİ)
                options.AddSecurityDefinition("X-Api-Key", new OpenApiSecurityScheme
                {
                    Name = "X-Api-Key",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "Şirketiniz için tanımlanan **API Key** değerini giriniz."
                });

                // ✅ 3. Dinamik Güvenlik Gereksinimi (Filter)
                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            return services;
        }
    }

    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // [Authorize] ve [ApiKey] niteliklerini kontrol et
#pragma warning disable CS8602 // Olası bir null başvurunun başvurma işlemi.
            var hasAuthorize = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                               context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
#pragma warning restore CS8602 // Olası bir null başvurunun başvurma işlemi.

#pragma warning disable CS8602 // Olası bir null başvurunun başvurma işlemi.
            var hasApiKey = context.MethodInfo.GetCustomAttributes(true).OfType<ApiKeyAttribute>().Any() ||
                             context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<ApiKeyAttribute>().Any();
#pragma warning restore CS8602 // Olası bir null başvurunun başvurma işlemi.

            if (hasAuthorize || hasApiKey)
            {
                operation.Security = new List<OpenApiSecurityRequirement>();

                // Eğer metot [Authorize] gerektiriyorsa JWT ekle
                if (hasAuthorize)
                {
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                            },
                            Array.Empty<string>()
                        }
                    });
                }

                // Eğer metot [ApiKey] gerektiriyorsa X-Api-Key ekle
                if (hasApiKey)
                {
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "X-Api-Key" }
                            },
                            Array.Empty<string>()
                        }
                    });
                }
            }
        }
    }
}