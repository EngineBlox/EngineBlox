using EngineBlox.Api;
using Moq;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace EngineBlox.Test.Utility.Api
{
    public static class JsonApiMock
    {
        public static Mock<IJsonApi<TApi>> ReturnDefault<TApi, TResult>(Expression<Func<IJsonApi<TApi>, Task<HttpResponseMessage>>> methodSetup) where TResult : new()
        {
            var api = new Mock<IJsonApi<TApi>>();
            var message = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new TResult()))
            };
            api.Setup(methodSetup).ReturnsAsync(message);
            return api;
        }
    }
}
