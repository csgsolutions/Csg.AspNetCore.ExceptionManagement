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
    /// <summary>
    /// App builder extensions
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Adds the Exception Management exception handler to the application builder using <see cref="ExceptionHandlerExtensions.UseExceptionHandler(IApplicationBuilder, Action{IApplicationBuilder})"/>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="allowUnsafeExceptions">A value of true indicates unsafe exceptions should be passed to the handler. The default value of false replaces any unsafe exception information with a generic message.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds exception management related services to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static ExceptionHandlerBuilder AddExceptionManagement(this IServiceCollection services, Action<ExceptionManagementOptions> setupAction = null)
        {
            if (setupAction != null)
            {
                services.Configure<ExceptionManagementOptions>(setupAction);
            }

            return new ExceptionHandlerBuilder(services);
        }

        /// <summary>
        /// Configures the use of the <see cref="Handlers.WebApiExceptionHandler(ExceptionContext)">WebApiExceptionHandler</see> handler, and a generic error response for unsafe exceptions.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ExceptionHandlerBuilder AddWebApiDefaults(this ExceptionHandlerBuilder builder)
        {
            builder.Services.Configure<ExceptionManagementOptions>(options =>
            {
                options.Handler = Handlers.WebApiExceptionHandler;
                options.UnsafeResult = ExceptionManagementOptions.GenericErrorResult;
            });

            return builder;
        }

        /// <summary>
        /// Adds a filter to the end of the exception management filter pipeline.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="filter">An exception filter.</param>
        /// <returns></returns>
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
