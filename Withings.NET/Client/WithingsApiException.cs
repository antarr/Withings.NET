using System;

namespace Withings.NET.Client
{
    public class WithingsApiException : Exception
    {
        public int StatusCode { get; }

        public WithingsApiException(int statusCode)
            : base($"Withings API Error: {statusCode}")
        {
            StatusCode = statusCode;
        }

        public WithingsApiException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public WithingsApiException(int statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
