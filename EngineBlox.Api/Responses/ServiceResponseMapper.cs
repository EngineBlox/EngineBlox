using EngineBlox.Api.Exceptions;
using EngineBlox.Responses;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EngineBlox.Api.Responses
{
    public static class ServiceResponseMapper
    {
        public static async Task<ServiceResponse> ToServiceResponse(this HttpResponseMessage response)
        {
            try
            {
                var serviceResponse = new ServiceResponse
                {
                    ServiceResult = response.StatusCode.ToServiceResult()
                };

                if (serviceResponse.IsFailure)
                {
                    serviceResponse.ErrorDetail = await response.Content.ReadAsStringAsync();
                }

                return serviceResponse;
            }
            catch (Exception ex)
            {
                return ServiceResponse.UnknownError(ex);
            }
        }

        public static async Task<ServiceResponse<TResult>> ToServiceResponse<TResult>(this HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<TResult>(body)
                        ?? throw new ApiException($"Expected to deserialise {nameof(TResult)} but null was returned");

                    return new ServiceResponse<TResult>(result);
                }
                catch (Exception ex)
                {
                    return ServiceResponse<TResult>.Failure(ServiceResult.UNKNOWN_ERROR, $"Unable to deserialise requested type of {typeof(TResult).Name}. {ex.GetType().Name}: {ex.Message}");
                }
            }
            else
            {
                return ServiceResponse<TResult>.Failure(response.StatusCode.ToServiceResult(), body);
            }
        }

        public static ServiceResult ToServiceResult(this HttpStatusCode statusCode) => (int)statusCode switch
        {
            200 => ServiceResult.SUCCESS,
            400 => ServiceResult.INVALID_OPERATION,
            404 => ServiceResult.RESOURCE_NOT_FOUND,
            500 => ServiceResult.UNKNOWN_ERROR,
            _ => ServiceResult.UNKNOWN_ERROR,
        };
    }
}
