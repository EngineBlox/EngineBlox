using EngineBlox.Azure.Functions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace EngineBlox.Test.Utility.Api
{
    public static class ActionResultExtensions
    {
        public static TResult GetSerialisedJsonReponse<TResult>(this IActionResult response)
        {
            var jsonResult = response.Should().BeOfType<SerialisedJsonResult>().Subject;

            return JsonConvert.DeserializeObject<TResult>(jsonResult.Content)
                ?? throw new Exception($"Expected to deserialise {typeof(TResult).Name}");
        }
    }
}
