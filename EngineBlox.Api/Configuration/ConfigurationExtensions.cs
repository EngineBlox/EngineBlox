using EngineBlox.Responses;
using Microsoft.Extensions.Configuration;

namespace EngineBlox.Api.Configuration
{
    public static class ConfigurationExtensions
    {
        public static string GetValueOrThrow(this IConfiguration configuration, string name)
        {
            var value = configuration.GetValue<string>(name);

            if (string.IsNullOrEmpty(value)) throw new ServiceException($"{name} is not present in configuration or has no value");

            return value;
        }
    }
}
