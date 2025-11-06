using Microsoft.Extensions.DependencyInjection;
using Users.Domain.Services.Class;
using Users.Domain.Services.Interface;

namespace Users.Domain.Dependency;

public static class DomainDependency
{
    public static IServiceCollection AddServices(this IServiceCollection services) {
        return services
            .AddScoped<IPerfilServices, PerfilServices>()
            .AddScoped<ILoggerServices, LoggerServices>()
            .AddScoped<IUserServices, UserServices>()
            .AddScoped<IJwtServices, JwtServices>()
            .AddScoped<ICarteiraServices, CarteiraServices>();

    }
}

