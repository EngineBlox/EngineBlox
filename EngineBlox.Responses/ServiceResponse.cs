using System;

namespace EngineBlox.Responses
{
    public enum ServiceResult
    {
        RESULT_UNDEFINED,
        SUCCESS,
        INVALID_OPERATION,
        VALIDATION_ERROR,
        RESOURCE_NOT_FOUND,
        UNKNOWN_ERROR
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public ServiceResponse(T value)
        {
            _value = value;
            ServiceResult = ServiceResult.SUCCESS;
        }

        private T _value;
        public T Value 
        { 
            get
            {
                return IsSuccess ? _value ?? throw new ServiceException($"No value was set for {typeof(T)}")
                                 : _value ?? throw new ServiceException($"Attempted to get result for unsuccessful request. Error {ErrorDetail}");
            }
            set => _value = value;
        }

        public static ServiceResponse<T> Failure(ServiceResult serviceResult, string errorDetail)
        {
            return new ServiceResponse<T>(default!) // see getter
            {
                ServiceResult = serviceResult,
                ErrorDetail = errorDetail
            };
        }
    }

    public class ServiceResponse
    {
        public ServiceResponse() { }

        public ServiceResponse(ServiceResult serviceResult, string errorDetail)
        {
            ServiceResult = serviceResult;
            ErrorDetail = errorDetail;
        }

        public static ServiceResponse Success => new ServiceResponse { ServiceResult = ServiceResult.SUCCESS };
        public static ServiceResponse InvalidOperation(string error) => new ServiceResponse { ServiceResult = ServiceResult.INVALID_OPERATION, ErrorDetail = error };
        public static ServiceResponse ResourceNotFound(string error) => new ServiceResponse { ServiceResult = ServiceResult.RESOURCE_NOT_FOUND, ErrorDetail = error };
        public static ServiceResponse UnknownError(string error) => new ServiceResponse { ServiceResult = ServiceResult.UNKNOWN_ERROR, ErrorDetail = error };
        public static ServiceResponse UnknownError(Exception ex) => new ServiceResponse { ServiceResult = ServiceResult.UNKNOWN_ERROR, ErrorDetail = ex.Message };

        public bool IsSuccess => ServiceResult == ServiceResult.SUCCESS;
        public bool IsFailure => ServiceResult != ServiceResult.SUCCESS;
        public ServiceResult ServiceResult { get; set; } = ServiceResult.RESULT_UNDEFINED;
        public string ErrorDetail { get; set; } = "";
    }
}
