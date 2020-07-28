using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Csg.AspNetCore.ExceptionManagement
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public class Handlers
    {
        private static readonly MediaTypeHeaderValue TextMediaType = MediaTypeHeaderValue.Parse("text/plain");
        private static readonly MediaTypeHeaderValue JsonMediaType = MediaTypeHeaderValue.Parse("application/json");

        /// <summary>
        /// This handler formats the error response as JSON or plain text depending on the value of the request's Accept header.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task WebApiExceptionHandler(ExceptionContext context)
        {
            var httpContext = context.HttpContext;
            var requestHeaders = httpContext.Request.GetTypedHeaders();

            httpContext.Response.StatusCode = context.Result.StatusCode;

            // If no accept header is sent, assume we want JSON I guess.
            if (requestHeaders.Accept == null || requestHeaders.Accept.Count == 0 || requestHeaders.Accept?.Any(x => JsonMediaType.IsSubsetOf(x)) == true)
            {
                httpContext.Response.ContentType = "application/json; chartset=utf8";

                // be compatible with https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails?view=aspnetcore-2.2
                var errorResponse = new
                {
                    Title = context.Result.ErrorTitle,
                    Detail = context.Result.ErrorDetail,
                    Extensions = new
                    {
                        ID = context.ErrorID,
                        Code = context.Result.ErrorCode,
                        Data = context.Result.ErrorData,
                    },
                    Status = context.Result.StatusCode,
                    Instance = context.HttpContext.Request.Path
                };

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
            else if (requestHeaders.Accept?.Any(x => TextMediaType.IsSubsetOf(x)) == true)
            {
                httpContext.Response.ContentType = "text/plain";

                await httpContext.Response.WriteAsync($"Error: {context.Result.ErrorTitle}\nDetail: {context.Result.ErrorDetail}");
            }
            else
            {
                //Should we write something here? Should
            }
        }
    }
}

