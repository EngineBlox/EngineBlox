using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EngineBlox.Azure.Functions
{
    public class SerialisedJsonResult : ContentResult
    {
        public SerialisedJsonResult(object value, bool camelCase = true)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            if (camelCase) options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            ContentType = MediaTypeNames.Application.Json;
            Content = JsonSerializer.Serialize(value, options);
        }
    }
}
