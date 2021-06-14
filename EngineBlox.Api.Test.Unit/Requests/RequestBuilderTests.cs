using EngineBlox.Api.Requests;
using EngineBlox.Responses;
using FluentAssertions;
using System;
using System.Net;
using Xunit;

namespace EngineBlox.Api.Test.Unit.Requests
{
    public class RequestBuilderTests
    {
        [Fact]
        public void GivenNoAdditionalCalls_WhenGetDefault_ThenSafeDefaults()
        {
            var defaultRequestBuilder = RequestBuilder.Default;

            defaultRequestBuilder.NamingStrategy.Should().Be(JsonNamingStrategy.Camel);
            defaultRequestBuilder.Headers.Should().NotBeNull().And.HaveCount(0);
            defaultRequestBuilder.UriSegmentParameters.Should().NotBeNull().And.HaveCount(0);
        }

        [Theory]
        [InlineData(JsonNamingStrategy.Camel)]
        [InlineData(JsonNamingStrategy.Pascal)]
        public void GivenNamingStrategyChosen_WhenUseJsonNamingStrategy_ThenStrategySet(JsonNamingStrategy namingStrategy)
        {
            var requestBuilder = new RequestBuilder().UseJsonNamingStrategy(namingStrategy);

            requestBuilder.NamingStrategy.Should().Be(namingStrategy);
        }

        [Fact]
        public void GivenHeaderDetails_WhenAddHeader_ThenHeaderAdded()
        {
            var requestBuilder = new RequestBuilder().AddHeader("TestHeader", "TestValue");

            requestBuilder.Headers.Should().Contain(h => h.Name == "TestHeader" && h.Value == "TestValue");
        }

        [Fact]
        public void GivenUriParameterDetails_WhenAddUriParameter_ThenUriParameterAdded()
        {
            var requestBuilder = new RequestBuilder().AddUriSegmentParameter("TestParam", "TestParamValue");

            requestBuilder.UriSegmentParameters.Should().Contain(h => h.Name == "TestParam" && h.Value == "TestParamValue");
        }

        [Fact]
        public void GivenQueryParameter_WhenAddQueryParameter_ThenQueryParameterAdded()
        {
            var requestBuilder = new RequestBuilder().AddQueryParameter("QueryParam", "QueryValue");

            requestBuilder.QueryParameters.Should().Contain(q => q.Name == "QueryParam" & q.Value == "QueryValue");
        }

        [Fact]
        public void GivenValidUriComponents_AndNoParameterisation_WhenBuildUri_ThenUriReturned()
        {
            var requestBuilder = RequestBuilder.Default;

            var uri = requestBuilder.BuildUri(new Uri("http://www.test.co.uk"), "/orders");

            uri.Should().Be("http://www.test.co.uk/orders");
        }

        [Fact]
        public void GivenValidUriComponents_AndParameterisation_WhenBuildUri_ThenUriReturned()
        {
            var requestBuilder = new RequestBuilder().AddUriSegmentParameter("orderId", "332");

            var uri = requestBuilder.BuildUri(new Uri("http://www.test.co.uk"), "/orders/{orderId}");

            uri.Should().Be("http://www.test.co.uk/orders/332");
        }

        [Fact]
        public void GivenUriSegmentParameter_AndNotInRelativeUri_WhenBuildUri_ThenHelpfulError()
        {
            var requestBuilder = new RequestBuilder().AddUriSegmentParameter("orderId", "332");

            Action buildUri = () => requestBuilder.BuildUri(new Uri("http://www.test.co.uk"), "/orders");

            buildUri.Should().Throw<ServiceException>()
                .WithMessage("Invalid request in api. Requested to replace {orderId} with 332 in uri /orders but {orderId} is not present");
        }

        [Fact]
        public void GivenQueryParameters_AndUriWithNoQueryParameters_WhenParameteriseAndGetUri_ThenUriReturned()
        {
            var requestBuilder = new RequestBuilder().AddQueryParameter("startDate", "1920-01-34 spaces here")
                                                     .AddQueryParameter("end-date_2", "2043-12-05T00:00:00");

            var uri = requestBuilder.BuildUri(new Uri("http://www.test.co.uk"), "/orders");

            uri.Should().Be("http://www.test.co.uk/orders?startDate=1920-01-34+spaces+here&end-date_2=2043-12-05T00%3A00%3A00");
            WebUtility.UrlDecode(uri.ToString()).Should().Be("http://www.test.co.uk/orders?startDate=1920-01-34 spaces here&end-date_2=2043-12-05T00:00:00");
        }
    }
}
