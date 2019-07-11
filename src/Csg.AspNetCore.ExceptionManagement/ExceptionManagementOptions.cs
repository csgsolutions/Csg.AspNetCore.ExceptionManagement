using System.Collections.Generic;

namespace Csg.AspNetCore.ExceptionManagement
{
    /// <summary>
    /// Represents the options for the exception management middleware.
    /// </summary>
    public class ExceptionManagementOptions
    {
        /// <summary>
        /// Provides a generic error message.
        /// </summary>
        public static readonly ExceptionResult GenericErrorResult = new ExceptionResult()
        {
            IsSafe = true,
            ErrorTitle = SR.GenericErrorTitle,
            ErrorDetail = SR.GenericErrorDetail,
            StatusCode = 500
        };

        /// <summary>
        /// Gets or sets the handler that will be used to return a response to the caller.
        /// </summary>
        public ExceptionHandlerDelegate Handler { get; set; }

        /// <summary>
        /// Gets or sets the list which defines the pipeline of exception filters.
        /// </summary>
        public List<ExceptionFilterDelegate> Filters { get; set; } = new List<ExceptionFilterDelegate>();

        /// <summary>
        /// Gets or sets the exception result that will be used when a generic error message is used in lieu of the actual message.
        /// </summary>
        public ExceptionResult UnsafeResult { get; set; }
    }
}
