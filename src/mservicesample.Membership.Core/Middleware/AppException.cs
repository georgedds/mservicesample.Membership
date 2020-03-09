using System;
using System.Net;

namespace mservicesample.Membership.Core.Middleware
{
    public class AppException : Exception
    {
        public AppException(HttpStatusCode code, object errors = null)
        {
            Code = code;
            Errors = errors;
        }

        public AppException( object errors = null)
        {
            Errors = errors;
        }

        public object Errors { get; set; }

        public HttpStatusCode Code { get; }
    }
}
