using EngineBlox.Api.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net.Http;
using Xunit;

namespace EngineBlox.Api.Test.Unit.Configuration
{
    public class StartupTests
    {
        [Fact]
        public void GivenStartup_WhenAddJsonApi_ThenIHttpClientFactoryAdded()
        {
            var provider = GivenStartupAdds<IOrders, Orders>();

            var factory = provider.GetRequiredService<IHttpClientFactory>();

            factory.Should().NotBeNull();
        }

        public static ServiceProvider GivenStartupAdds<TApi, TImp>()
        {
            var services = new ServiceCollection();

            services.AddJsonApi<TApi, TImp>();

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new HttpClient());
            services.AddTransient(_ => factoryMock.Object);

            services.AddSingleton(_ => Orders.BuildConfiguration());

            return services.BuildServiceProvider();
        }

        [Fact]
        public void GivenStartup_WhenAddJsonApi_ThenApiDefinitionAvailable()
        {
            var provider = GivenStartupAdds<IOrders, Orders>();

            var definition = provider.GetRequiredService<IApiDefinition<IOrders>>();

            definition.Should().NotBeNull();
        }

        [Fact]
        public void GivenStartup_WhenAddJsonApi_ThenJsonApiAvailable()
        {
            var provider = GivenStartupAdds<IOrders, Orders>();

            var ordersApi = provider.GetRequiredService<IJsonApi<IOrders>>();

            ordersApi.Should().NotBeNull();
        }

        [Fact]
        public void GivenStartup_WhenAddJsonApi_ThenApiAvailable()
        {
            var provider = GivenStartupAdds<IOrders, Orders>();

            var ordersApi = provider.GetRequiredService<IOrders>();

            ordersApi.Should().NotBeNull();
        }
    }
}
