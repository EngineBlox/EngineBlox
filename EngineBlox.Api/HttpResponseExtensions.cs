using EngineBlox.Api.Exceptions;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EngineBlox.Api
{
    public static class HttpResponseExtensions
    {
        public static async Task<TResult> GetResultAsync<TResult>(this HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync())
                ?? throw new ApiException($"Attempted to deserialise json api result {nameof(TResult)} but null was returned");
        }

        public static string GetSingleHeader(this HttpResponseMessage response, string name)
        {
            var values = response.Headers.GetValues(name);

            if (values.Count() > 1) throw new ApiException($"Expected 1 header matching {name} but received {values.Count()}");
            // 0?
            return values.First();
        }

        public static async Task EnsureSuccessOrThrowWithBody(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    throw new ApiException($"{response.StatusCode}: {error}.");
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}
