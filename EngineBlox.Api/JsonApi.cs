using EngineBlox.Api.Configuration;
using EngineBlox.Api.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EngineBlox.Api
{
    public interface IJsonApi<TApi>
    {
        Task<HttpResponseMessage> GetAsync(RequestBuilder? requestBuilder = null, [CallerMemberName] string memberName = "");
        Task<TResult> GetOrThrowAsync<TResult>(RequestBuilder? requestBuilder = null, [CallerMemberName] string memberName = "");

        Task<HttpResponseMessage> PostAsync<TPayload>(TPayload payload, RequestBuilder? requestBuilder = null, [CallerMemberName] string memberName = "");
        Task PostOrThrowAsync<TPayload>(TPayload payload, RequestBuilder? requestBuilder = null, [CallerMemberName] string memberName = "");
    }

    public class JsonApi<TApi> : IJsonApi<TApi>
    {
        private readonly HttpClient _client;
        private readonly IApiDefinition<TApi> _apiDefinition;

        public JsonApi(IHttpClientFactory factory, IApiDefinition<TApi> apiDefinition)
        {
            _client = factory.CreateClient(nameof(TApi));
            _client.BaseAddress = apiDefinition.BaseAddress;
            _apiDefinition = apiDefinition;
        }

        public async Task<HttpResponseMessage> GetAsync(RequestBuilder? requestBuilder = null, [CallerMemberName] string memberName = "") 
            => await _client.GetAsync(BuildRequest(requestBuilder, memberName));

        public async Task<TResult> GetOrThrowAsync<TResult>(RequestBuilder? requestBuilder = null, [CallerMemberName] string memberName = "")
        {
            var response = await _client.GetAsync(BuildRequest(requestBuilder, memberName));

            await response.EnsureSuccessOrThrowWithBody();

            return await response.GetResultAsync<TResult>();
        }

        public async Task<HttpResponseMessage> PostAsync<TPayload>(TPayload payload, RequestBuilder? requestBuilder = null, [CallerMemberName] string memberName = "")
        {
            return await PostInternal(payload, requestBuilder, memberName);
        }

        public async Task PostOrThrowAsync<TPayload>(TPayload payload, RequestBuilder? requestBuilder = null, [CallerMemberName] string memberName = "")
        {
            var response = await PostInternal(payload, requestBuilder, memberName);

            await response.EnsureSuccessOrThrowWithBody();
        }

        private Uri BuildRequest(RequestBuilder? requestBuilder, string memberName)
        {
            if (requestBuilder is null) requestBuilder = RequestBuilder.Default;

            return requestBuilder.BuildUri(_apiDefinition.BaseAddress, _apiDefinition.GetRelativeUri(memberName));
        }

        private async Task<HttpResponseMessage> PostInternal<TPayload>(TPayload payload, RequestBuilder? requestBuilder, string memberName)
        {
            if (requestBuilder is null) requestBuilder = RequestBuilder.Default;

            var json = JsonConvert.SerializeObject(payload, new JsonSerializerSettings { ContractResolver = CreateContractResolver(requestBuilder.NamingStrategy) });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            foreach (var header in requestBuilder.Headers)
            {
                content.Headers.Add(header.Name, header.Value);
            }

            var uri = requestBuilder.BuildUri(_apiDefinition.BaseAddress, _apiDefinition.GetRelativeUri(memberName));
            return await _client.PostAsync(uri, content);
        }

        private DefaultContractResolver CreateContractResolver(JsonNamingStrategy namingStrategy)
            => namingStrategy is JsonNamingStrategy.Camel ? new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }
                                                          : new DefaultContractResolver();
    }
}
