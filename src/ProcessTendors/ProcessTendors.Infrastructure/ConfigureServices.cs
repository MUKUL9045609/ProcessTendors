using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessTendors.Application.Common.Interfaces.Repositories;
using ProcessTendors.Application.Common.Interfaces.Service;
using ProcessTendors.Infrastructure.Interfaces;
using ProcessTendors.Infrastructure.Interfaces.Repositories;

namespace ProcessTendors.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IScraperService, ScraperService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthRepositories, AuthRepositories>();
            return services;
        }
    }
}
