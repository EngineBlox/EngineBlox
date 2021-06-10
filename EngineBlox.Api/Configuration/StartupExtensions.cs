using Microsoft.Extensions.DependencyInjection;

namespace EngineBlox.Api.Configuration
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddJsonApi<TApi, TImplementation>(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddSingleton<IApiDefinition<TApi>, ApiDefinition<TApi>>();
            services.AddSingleton<IJsonApi<TApi>, JsonApi<TApi>>();
            services.AddScoped(typeof(TApi), typeof(TImplementation));

            return services;
        }
    }
}
