using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Csg.AspNetCore.ExceptionManagement
{
    /// <summary>
    /// Represents a function that handles exceptions by writing to or manipulating the response.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task ExceptionHandlerDelegate(ExceptionContext context);

    /// <summary>
    /// Represents an exception filter that can set an <see cref="ExceptionResult"/> or manipulate an existing one.
    /// </summary>
    /// <param name="context"></param>
    public delegate void ExceptionFilterDelegate(ExceptionContext context);

    /// <summary>
    /// Encapulates the current http request, the thrown exception, and the result.
    /// </summary>
    public class ExceptionContext
    {
        /// <summary>
        /// Gets or sets the exception that was thrown causing the exception middleware to be triggered.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier that can uniquely identify this exception.
        /// </summary>
        public string ErrorID { get; set; }

        /// <summary>
        /// Gets or sets the HttpContext represnting the current request.
        /// </summary>
        public HttpContext HttpContext { get; set; }

        /// <summary>
        /// Gets or sets the exception result. If no exception filter sets this value, one will be automatically created fron the <see cref="Error"/>
        /// </summary>
        public ExceptionResult Result { get; set; }

        /// <summary>
        /// Gets a reference to the configured exception mangement options.
        /// </summary>
        public ExceptionManagementOptions Options { get; set; }
    }
}
