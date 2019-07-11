using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Csg.AspNetCore.ExceptionManagement
{
    public class ExceptionHandlerBuilder
    {
        public ExceptionHandlerBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        public IServiceCollection Services { get; private set; }
    }

    public class Filters
    {
        public static void UnsafeExceptionFilter(ExceptionContext context)
        {
            if (!context.Result.IsSafe)
            {
                if (context.Options.UnsafeExceptionResult == null)
                {
                    throw new Exception($"The configuration option {nameof(context.Options.UnsafeExceptionResult)} is null");
                }

                context.Result = context.Options.UnsafeExceptionResult;
            }
        }
    }

    public class Handlers
    {
        private static readonly MediaTypeHeaderValue TextMediaType = MediaTypeHeaderValue.Parse("text/plain");
        private static readonly MediaTypeHeaderValue JsonMediaType = MediaTypeHeaderValue.Parse("application/json");

        public static async Task WebApiExceptionHandler(ExceptionContext context)
        {
            var httpContext = context.HttpContext;
            var requestHeaders = httpContext.Request.GetTypedHeaders();

            httpContext.Response.StatusCode = context.Result.StatusCode;

            if (requestHeaders.Accept.Any(x => JsonMediaType.IsSubsetOf(x)))
            {
                httpContext.Response.ContentType = "application/json; chartset=utf8";

                var errorResponse = new
                {
                    ErrorID = context.ErrorID,
                    Title = context.Result.ErrorTitle,
                    Detail = context.Result.ErrorDetail
                };

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
            else if (requestHeaders.Accept.Any(x => TextMediaType.IsSubsetOf(x)))
            {
                httpContext.Response.ContentType = "text/plain";

                await httpContext.Response.WriteAsync($"Error: {context.Result.ErrorTitle}\nDetail: ${context.Result.ErrorDetail}");
            }
        }
    }
}

