using System;
using System.Runtime.Serialization;

namespace EngineBlox.Responses
{
    [Serializable]
    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message)
        {
        }

        protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
