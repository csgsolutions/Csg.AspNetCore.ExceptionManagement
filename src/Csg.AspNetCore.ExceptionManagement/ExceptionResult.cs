using System;
using System.Text;
using System.Threading.Tasks;

namespace Csg.AspNetCore.ExceptionManagement
{
    /// <summary>
    /// Represents an exception that will be sent to the caller.
    /// </summary>
    public class ExceptionResult
    {
        /// <summary>
        /// Gets or sets the HTTP Status code to use in the response.
        /// </summary>
        public int StatusCode { get; set; } = 500;

        /// <summary>
        /// Gets or sets the error title that will be used.
        /// </summary>
        public string ErrorTitle { get; set; }

        /// <summary>
        /// Gets or sets the error message detail that can be used by the exception handler to provide additional information to the caller.
        /// </summary>
        public string ErrorDetail { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if this error message is safe to send to callers in a production environment.
        /// </summary>
        public bool IsSafe { get; set; } = false;

        /// <summary>
        /// Creates a new result from the given exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static ExceptionResult Create(Exception ex)
        {
            return new ExceptionResult()
            {
                ErrorTitle = ex.Message,
                ErrorDetail = ex.StackTrace
            };
        }

    }
}
