using System;

namespace Insolvency.Common.Exceptions
{
    public abstract class HttpResponseException : Exception
    {
        public abstract int StatusCode { get; }

        public abstract override string Message { get; }
    }
}
