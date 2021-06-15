using EngineBlox.Responses;
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
                ?? throw new ServiceException($"Attempted to deserialise json api result {nameof(TResult)} but null was returned");
        }

        public static string GetSingleHeader(this HttpResponseMessage response, string name)
        {
            var values = response.Headers.GetValues(name);

            if (values.Count() != 1) throw new ServiceException($"Expected 1 header matching {name} but received {values.Count()}");
            var value = values.First();
            if (string.IsNullOrEmpty(value)) throw new ServiceException($"Expected value for header {name} but got null or empty");

            return values.First();
        }

        public static async Task EnsureSuccessOrThrowWithBody(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    throw new ServiceException($"{response.StatusCode}: {error}.", (int)response.StatusCode);
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}
