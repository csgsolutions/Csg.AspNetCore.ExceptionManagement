using Microsoft.Extensions.DependencyInjection;

namespace Csg.AspNetCore.ExceptionManagement
{
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public class ExceptionHandlerBuilder
    {
        public ExceptionHandlerBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        public IServiceCollection Services { get; private set; }
    }
}

