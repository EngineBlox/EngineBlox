using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EngineBlox.Azure.Functions
{
    public class SerialisedJsonResult : ContentResult
    {
        public SerialisedJsonResult(object value)
        {
            ContentType = MediaTypeNames.Application.Json;
            Content = JsonSerializer.Serialize(value, Options);
        }

        public static JsonSerializerOptions Options => new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
    }
}
