using EngineBlox.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;

namespace EngineBlox.Azure.Functions
{
    public static class ResponseHandler
    {
        public static IActionResult NotFound(string message) => Handle(ServiceResponse.ResourceNotFound(message));

        public static IActionResult Handle(this ServiceResponse serviceResponse)
        {
            if (serviceResponse.IsSuccess)
            {
                return new StatusCodeResult(200);
            }
            else
            {
                return new ObjectResult(MapError(serviceResponse)) { StatusCode = MapStatusCode(serviceResponse.ServiceResult) };
            }
        }

        public static IActionResult Handle<TResult>(this TResult result)
        {
            if (result is null) return new OkResult();

            return new SerialisedJsonResult(result)
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public static IActionResult Handle(this Exception ex) => Handle(ServiceResponse.UnknownError(ex));

        public static IActionResult Handle(this ServiceException ex) => ex.ErrorCode switch
        {
            400 => Handle(ServiceResponse.InvalidOperation(ex.Message)),
            404 => Handle(ServiceResponse.ResourceNotFound(ex.Message)),
            _ => Handle(ServiceResponse.UnknownError(ex.Message)),
        };

        public static ProblemDetails MapError(ServiceResponse serviceResponse) => new ProblemDetails
        {
            Title = serviceResponse.ServiceResult.ToString(),
            Status = MapStatusCode(serviceResponse.ServiceResult),
            Detail = serviceResponse.ErrorDetail
        };

        public static readonly JsonSerializerSettings JsonOptions =
            new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                Converters = { new StringEnumConverter() },
                Formatting = Formatting.Indented
            };

        private static int MapStatusCode(ServiceResult serviceResponse) => serviceResponse switch
        {
            ServiceResult.RESULT_UNDEFINED => 500,
            ServiceResult.SUCCESS => 200,
            ServiceResult.INVALID_OPERATION => 400,
            ServiceResult.VALIDATION_ERROR => 400,
            ServiceResult.RESOURCE_NOT_FOUND => 404,
            ServiceResult.UNKNOWN_ERROR => 500,
            _ => 500
        };
    }
}
