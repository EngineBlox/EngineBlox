using EngineBlox.Api.Configuration;
using EngineBlox.Api.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace EngineBlox.Api.Test.Unit.Configuration
{
    public class ApiDefinitionTests
    {
        [Fact]
        public void GivenInterface_WithIPrefix_WhenGetApiName_ThenNameReturned_WithoutIPrefix()
        {
            var definition = new ApiDefinition<IOrders>(Orders.BuildConfiguration());

            definition.ApiName.Should().Be("Orders");
        }

        [Fact]
        public void GivenInterface_WithoutIPrefix_WhenGetApiName_ThenNameReturned()
        {
            var definition = new ApiDefinition<OrdersWithoutPrefixI>(
                Orders.BuildConfiguration("Api:OrdersWithoutPrefixI:BaseAddress", "http://www.test.co.uk"));

            definition.ApiName.Should().Be("OrdersWithoutPrefixI");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Violation for unit test")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Violation for unit test")]
        public interface OrdersWithoutPrefixI { }

        [Fact]
        public void GivenApiDefinition_WithValidConfig_WhenGetBaseAddress_ThenBaseAddressReturned()
        {
            var definition = new ApiDefinition<IOrders>(
                Orders.BuildConfiguration("Api:Orders:BaseAddress", "http://www.test.co.uk"));

            definition.BaseAddress.Should().Be("http://www.test.co.uk");
        }

        [Fact]
        public void GivenInvalidBaseAddress_WhenConstructApiDefinition_ThenHelpfulError()
        {
            Action addApiInStartup = () => new ApiDefinition<IOrders>(
                Orders.BuildConfiguration("Api:Orders:BaseAddress", "invalidbaseaddress"));

            addApiInStartup.Should().Throw<ApiException>()
                .WithMessage("Base address for api \"Orders\" from config \"Api:Orders:BaseAddress\" is not a valid Uri");
        }

        [Theory]
        [InlineData("Api:Orders:GetOrderSummaries", "/orders", "GetOrderSummaries")]
        [InlineData("Api:Orders:StartPicking", "/orders/{orderId}", "StartPicking")]
        [InlineData("Api:Orders:SubmitPick", "/orders/{orderId}", "SubmitPick")]
        [InlineData("Api:Orders:NoAsyncSuffixTest", "/no/async/test", "NoAsyncSuffixTest")]
        public void GivenApiDefinition_WithValidConfig_WhenGetEndpoint_ThenEndpointReturned(string configName, string endpoint, string expectedEndpointName)
        {
            var definition = new ApiDefinition<IOrders>(
                Orders.BuildConfiguration(configName, endpoint));

            definition.Endpoints.Should().Contain(e => e.Name == expectedEndpointName &&
                                                       e.Uri == endpoint);
        }

        [Fact]
        public void GivenInvalidEndpoint_WhenConstructApiDefinition_ThenHelpfulError()
        {
            Action addApiInStartup = () => new ApiDefinition<IOrders>(
                Orders.BuildConfiguration("Api:Orders:SubmitPick", "http:\\\\//$\\*not-a-valid-relative-uri"));

            addApiInStartup.Should().Throw<ApiException>()
                .WithMessage("Unable to combine api \"Orders\" base address \"http://www.test.co.uk/\" with endpoint \"SubmitPick\" value \"http:\\\\//$\\*not-a-valid-relative-uri\" from configuration \"Api:Orders:SubmitPick\" into a valid Uri");
        }

        [Theory]
        [InlineData("SubmitPick", "/orders/{orderId}")]
        [InlineData("SubmitPickAsync", "/orders/{orderId}")]
        [InlineData("GetOrderSummaries", "/orders")]
        public void GivenEndpointExists_WhenGetRelativeUri_ThenUriReturned(string endpointName, string expectedUri)
        {
            var definition = new ApiDefinition<IOrders>(Orders.BuildConfiguration());

            definition.GetRelativeUri(endpointName).Should().Be(expectedUri);
        }
    }
}
