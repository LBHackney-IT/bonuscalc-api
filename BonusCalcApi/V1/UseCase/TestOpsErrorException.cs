using System;
using System.Runtime.Serialization;

namespace BonusCalcApi.V1.UseCase
{
    [Serializable]
    public class TestOpsErrorException : Exception
    {
        public TestOpsErrorException()
        {
        }

        public TestOpsErrorException(string message) : base(message)
        {
        }

        public TestOpsErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TestOpsErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
