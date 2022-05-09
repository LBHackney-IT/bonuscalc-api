using System;
using BonusCalcApi.V1.Exceptions;

namespace BonusCalcApi
{
    public static class ThrowHelper
    {
        public static void ThrowNotFound(string message) => throw new ResourceNotFoundException(message);
        public static void ThrowUnsupported(string message) => throw new NotSupportedException(message);
        public static void ThrowUnauthorizedAccessException(string message) => throw new UnauthorizedAccessException(message);
    }
}
