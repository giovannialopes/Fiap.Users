using Microsoft.Extensions.DependencyInjection;
using Users.Domain.Repositories;
using Users.Infrastructure.Repositories;

namespace Users.Infrastructure.Dependency;

public static class InfrastructureDependency
{
    public static IServiceCollection AddRepositories(this IServiceCollection services) {
        return services
            .AddScoped<IPerfilRepository, PerfilRepository>()
            .AddScoped<ICarteiraRepository, CarteiraRepository>()
            .AddScoped<ILoggerRepository, LoggerRepository>();

    }

}