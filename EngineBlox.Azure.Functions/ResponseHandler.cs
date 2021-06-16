using EngineBlox.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;
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

        public static IActionResult Handle(this Exception ex)
        {
            var exType = ex.GetType();

            if (exType == typeof(ApiException))
                return Handle((ex as ApiException)!);

            if (exType == typeof(ServiceException))
                return Handle((ex as ServiceException)!);

            return Handle(ServiceResponse.UnknownError(ex));
        }

        public static IActionResult Handle(this ApiException ex)
        {
            bool shouldPropagateErrorCode = ex.StatusCode != HttpStatusCode.OK;

            return new ObjectResult(new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = shouldPropagateErrorCode ? $"{ex.StatusCode}" : $"{HttpStatusCode.InternalServerError}",
                Status = shouldPropagateErrorCode ? (int)ex.StatusCode : 500,
                Detail = JsonConvert.SerializeObject(new
                {
                    ex.Message,
                    ex.Uri
                })
            }) { StatusCode = shouldPropagateErrorCode ? (int)ex.StatusCode : 500 };
        }

        public static IActionResult Handle(this ServiceException ex) => Handle(ex.ErrorCode, ex.Message);

        public static IActionResult Handle(int errorCode, string message) => errorCode switch
        {
            400 => Handle(ServiceResponse.InvalidOperation(message)),
            404 => Handle(ServiceResponse.ResourceNotFound(message)),
            _ => Handle(ServiceResponse.UnknownError(message)),
        };

        public static Microsoft.AspNetCore.Mvc.ProblemDetails MapError(ServiceResponse serviceResponse) => new Microsoft.AspNetCore.Mvc.ProblemDetails
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
