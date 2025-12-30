using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application katmanındaki tüm AutoMapper profillerini otomatik bulur ve kaydeder
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}