using System;

namespace Csg.AspNetCore.ExceptionManagement
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public class Filters
    {
        /// <summary>
        /// This filter sets the <see cref="ExceptionContext.Result">exception result</see> to the value of <see cref="ExceptionManagementOptions.UnsafeResult"/> if the existing result is not marked as safe.
        /// </summary>
        /// <param name="context"></param>
        public static void UnsafeExceptionFilter(ExceptionContext context)
        {
            if (!(context.Result?.IsSafe == true))
            {
                if (context.Options.UnsafeResult == null)
                {
                    throw new Exception($"The configuration option {nameof(context.Options.UnsafeResult)} is null.");
                }

                context.Result = context.Options.UnsafeResult;
            }
        }
    }
}

