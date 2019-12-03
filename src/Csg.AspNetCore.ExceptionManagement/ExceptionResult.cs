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
        /// Gets or sets a string that can be used to provide additional error detail. 
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets an object that can be used to provide additional error detail. 
        /// </summary>
        public object ErrorData { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if this error message is safe to send to callers in a production environment.
        /// </summary>
        public bool IsSafe { get; set; } = false;

        /// <summary>
        /// Creates a new result from the given exception
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="isSafe">Specifies whether the result will be marked as safe to return detailed information to the caller.</param>
        /// <param name="errorCode">An application specific error code.</param>
        /// <param name="errorData">An application specific error object.</param>
        /// <param name="statusCode">The HTTP status code to use in the response. The default value is 500 if not specified.</param>
        /// <returns></returns>
        public static ExceptionResult Create(Exception ex, bool isSafe = false, string errorCode = null, object errorData = null, int? statusCode = null)
        {
            var result = new ExceptionResult()
            {
                ErrorTitle = ex.Message,
                ErrorDetail = ex.StackTrace,
                ErrorCode = errorCode,
                ErrorData = errorData,
                IsSafe = isSafe
            };

            if (statusCode.HasValue)
            {
                result.StatusCode = statusCode.Value;
            }

            return result;
        }

    }
}
