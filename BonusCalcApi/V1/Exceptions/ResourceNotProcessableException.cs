using System;
using System.Runtime.Serialization;

namespace BonusCalcApi.V1.Exceptions
{
    [Serializable]
    public class ResourceNotProcessableException : Exception
    {
        public ResourceNotProcessableException()
        {
        }

        public ResourceNotProcessableException(string message) : base(message)
        {
        }

        public ResourceNotProcessableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ResourceNotProcessableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
