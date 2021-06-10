using EngineBlox.Api.Responses;
using EngineBlox.Responses;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EngineBlox.Api.Test.Unit.Responses
{
    public class ServiceResponseMapperTests
    {
        [Fact]
        public async Task GivenSuccessfulResponse_WhenToServiceResponse_ThenSuccessReturned()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            var serviceResponse = await response.ToServiceResponse();

            serviceResponse.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GivenNonSuccessResponse_WhenToServiceResponse_ThenFailureReturned_WithBody()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Something bad happened")
            };

            var serviceResponse = await response.ToServiceResponse();

            serviceResponse.IsSuccess.Should().BeFalse();
            serviceResponse.ServiceResult.Should().Be(ServiceResult.INVALID_OPERATION);
            serviceResponse.ErrorDetail.Should().Be("Something bad happened");
        }

        [Fact]
        public async Task GivenSuccessulResponse_WithTResult_WhenToServiceResponse_ThenSuccessReturned_WithTResult()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ImportantObject { Id = 5 }))
            };

            var serviceResponse = await response.ToServiceResponse<ImportantObject>();

            serviceResponse.IsSuccess.Should().BeTrue();
            serviceResponse.Value.Id.Should().Be(5);
        }
        public class ImportantObject { public int Id { get; set; } }

        [Fact]
        public async Task GivenNonSuccessResponse_OfTResult_WhenToServiceResponse_ThenFailureReturned_WithBody()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Something bad happened")
            };

            var serviceResponse = await response.ToServiceResponse<ImportantObject>();

            serviceResponse.IsSuccess.Should().BeFalse();
            serviceResponse.ServiceResult.Should().Be(ServiceResult.INVALID_OPERATION);
            serviceResponse.ErrorDetail.Should().Be("Something bad happened");
        }

        [Fact]
        public async Task GivenSuccessulResponse_WithExpectedModelMissing_WhenToServiceResponse_ThenHelpfulError()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("This isn't the reponse we expected")
            };

            var serviceResponse = await response.ToServiceResponse<ImportantObject>();

            serviceResponse.IsSuccess.Should().BeFalse();
            serviceResponse.ErrorDetail.Should().Be(
                "Unable to deserialise requested type of ImportantObject. " +
                "JsonReaderException: Unexpected character encountered while parsing value: T. Path '', line 0, position 0.");
        }

        [Theory]
        [InlineData(HttpStatusCode.OK, ServiceResult.SUCCESS)]
        [InlineData(HttpStatusCode.BadRequest, ServiceResult.INVALID_OPERATION)]
        [InlineData(HttpStatusCode.NotFound, ServiceResult.RESOURCE_NOT_FOUND)]
        [InlineData(HttpStatusCode.UnprocessableEntity, ServiceResult.UNKNOWN_ERROR)]
        [InlineData(HttpStatusCode.BadGateway, ServiceResult.UNKNOWN_ERROR)]
        public void GivenHttpStatusCode_WhenToServiceResult_ThenServiceResultMapped(HttpStatusCode httpStatusCode, ServiceResult expectedServiceResult)
        {
            httpStatusCode.ToServiceResult().Should().Be(expectedServiceResult);
        }
    }
}