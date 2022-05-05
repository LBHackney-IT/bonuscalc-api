using System;
using System.IO;
using System.Threading.Tasks;
using BonusCalcApi.V1.Exceptions;
using Microsoft.AspNetCore.Http;

namespace BonusCalcApi.V1.MiddleWare
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ResourceNotFoundException e)
            {
                await httpContext.SetResponse(404, e.Message);
            }
            catch (NotSupportedException e)
            {
                await httpContext.SetResponse(400, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                await httpContext.SetResponse(401, e.Message);
            }
        }
    }

    public static class ContextExtensions
    {
        public static async Task SetResponse(this HttpContext httpContext, int code, string message)
        {
            await using var writer = new StreamWriter(httpContext.Response.Body);
            httpContext.Response.StatusCode = code;
            httpContext.Response.ContentType = "text/plain";
            await writer.WriteAsync(message);
            await writer.FlushAsync();
        }
    }
}
