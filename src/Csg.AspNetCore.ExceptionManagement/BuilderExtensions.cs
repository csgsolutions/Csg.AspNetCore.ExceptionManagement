using Csg.AspNetCore.ExceptionManagement;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Microsoft.AspNetCore.Builder
{
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseExceptionManagement(this IApplicationBuilder app, bool allowUnsafeExceptions = false)
        {
            return app.UseExceptionHandler(a => a.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                var options = context.RequestServices.GetRequiredService<IOptions<ExceptionManagementOptions>>().Value;
                var exceptionContext = new ExceptionContext()
                {
                    Error = feature.Error,
                    ErrorID = System.Guid.NewGuid().ToString(),
                    HttpContext = context,
                    Options = options
                };

                foreach (var filter in options.Filters)
                {
                    filter.Invoke(exceptionContext);
                }

                if (exceptionContext.Result == null)
                {
                    exceptionContext.Result = ExceptionResult.Create(exceptionContext.Error);
                }

                if (!allowUnsafeExceptions)
                {
                    Filters.UnsafeExceptionFilter(exceptionContext);
                }

                await options.Handler.Invoke(exceptionContext);
            }));
        }

        public static ExceptionHandlerBuilder AddExceptionManagement(this IServiceCollection services, Action<ExceptionManagementOptions> setupAction = null)
        {
            if (setupAction != null)
            {
                services.Configure<ExceptionManagementOptions>(setupAction);
            }

            return new ExceptionHandlerBuilder(services);
        }

        public static ExceptionHandlerBuilder AddWebApiDefaults(this ExceptionHandlerBuilder builder)
        {
            builder.Services.Configure<ExceptionManagementOptions>(options =>
            {
                options.Handler = Handlers.WebApiExceptionHandler;
                options.UnsafeExceptionResult = ExceptionManagementOptions.GenericErrorResult;
            });

            return builder;
        }

        public static ExceptionHandlerBuilder AddFilter(this ExceptionHandlerBuilder builder, ExceptionFilterDelegate filter)
        {
            builder.Services.Configure<ExceptionManagementOptions>(options =>
            {
                options.Filters.Add(filter);
            });

            return builder;
        }
    }
}
