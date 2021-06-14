using EngineBlox.Responses;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace EngineBlox.Api.Configuration
{
    public interface IApiDefinition<TApi>
    {
        string ApiName { get; }
        Uri BaseAddress { get; }
        List<ApiEndpoint> Endpoints { get; }

        string GetRelativeUri(string endpointName);
    }

    public class ApiDefinition<TApi> : IApiDefinition<TApi>
    {
        private readonly IConfiguration _configuration;

        public ApiDefinition(IConfiguration configuration)
        {
            _configuration = configuration;

            ApiName = GetApiName();
            BaseAddress = GetAndValidateBaseAddress();
            Endpoints = GetAndValidateEndpoints();
        }

        public string ApiName { get; }
        public Uri BaseAddress { get; }
        public List<ApiEndpoint> Endpoints { get; }

        public string GetRelativeUri(string endpointName)
        {
            var endpoint = Endpoints.SingleOrDefault(e => e.Name == StripAsyncSuffix(endpointName));

            if (endpoint == null) throw new ServiceException($"No endpoint configuration found for {endpointName}");

            return endpoint.Uri;
        }

        private string StripAsyncSuffix(string memberName)
        {
            return memberName.EndsWith("Async") ? memberName[0..^5]
                                                : memberName;
        }

        private string GetApiName()
        {
            var name = typeof(TApi).Name;

            if (name.StartsWith("I"))
                name = name.Remove(0, 1);

            return name;
        }

        private Uri GetAndValidateBaseAddress()
        {
            Uri baseAddress;

            try
            {
                baseAddress = new Uri(_configuration.GetValueOrThrow($"Api:{ApiName}:BaseAddress"));
            }
            catch (UriFormatException ex)
            {
                throw new ServiceException($"Base address for api \"{ApiName}\" from config \"Api:{ApiName}:BaseAddress\" is not a valid Uri", ex);
            }

            return baseAddress;
        }

        private List<ApiEndpoint> GetAndValidateEndpoints()
        {
            var methods = typeof(TApi).GetMethods();

            var endpoints = new List<ApiEndpoint>();
            foreach (var method in methods)
            {
                string endpointName = StripAsyncSuffix(method.Name);

                string uri = GetAndValidateUri(endpointName);

                endpoints.Add(new ApiEndpoint(endpointName, uri));
            }

            return endpoints;
        }

        [SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "Used as a startup-time validation check")]
        private string GetAndValidateUri(string endpointName)
        {
            var relativeUri = _configuration.GetValueOrThrow($"Api:{ApiName}:{endpointName}");

            // We parameterise raw string later before converting to Uri, this is to verify endpoint forms a valid uri at startup time rather than run time
            try
            {
                var tempToValidate = new Uri(BaseAddress, relativeUri);
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Unable to combine api \"{ApiName}\" base address \"{BaseAddress}\" with endpoint \"{endpointName}\" value \"{relativeUri}\" from configuration \"Api:{ApiName}:{endpointName}\" into a valid Uri", ex);
            }

            return relativeUri;
        }
    }
}
