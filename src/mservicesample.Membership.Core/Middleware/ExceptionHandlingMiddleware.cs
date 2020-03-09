using Microsoft.AspNetCore.Http;
using mservicesample.Membership.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context); //continue
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            //todo handle custom errors
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected


            //var message = ex.Message;
            //if (ex is BadRequestException)
            //{
            //    code = HttpStatusCode.BadRequest;
            //}
            //else if (ex is NotFoundException)
            //{
            //    code = HttpStatusCode.NotFound;
            //}
            //else if (ex is NotAuthorizedException)
            //{
            //    code = HttpStatusCode.Forbidden;
            //}
            //else if (ex is NotAuthenticatedException)
            //{
            //    code = HttpStatusCode.Unauthorized;
            //}

            if (ex is AppException)
            {
                code = HttpStatusCode.UnprocessableEntity;
            }

            var result = JsonConvert.SerializeObject(new { error = ex.Message });
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }


    }
}
