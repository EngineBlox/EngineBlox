using System;
using System.Runtime.Serialization;

namespace EngineBlox.Responses
{
    [Serializable]
    public class ServiceException : Exception
    {
        public int ErrorCode { get; } = 500;

        public ServiceException(string message) : base(message){}

        public ServiceException(string message, int errorCode) : base(message){ ErrorCode = errorCode; }

        public ServiceException(string message, Exception innerException) : base(message, innerException) { }

        protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
