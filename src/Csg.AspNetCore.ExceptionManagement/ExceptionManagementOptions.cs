using System.Collections.Generic;

namespace Csg.AspNetCore.ExceptionManagement
{
    public class ExceptionManagementOptions
    {
        public static readonly ExceptionResult GenericErrorResult = new ExceptionResult()
        {
            IsSafe = true,
            ErrorTitle = "An error occurred",
            ErrorDetail = "An unknown error has occurred.",
            StatusCode = 500
        };

        public ExceptionHandlerDelegate Handler { get; set; }


        public List<ExceptionFilterDelegate> Filters { get; set; } = new List<ExceptionFilterDelegate>();

        public ExceptionResult UnsafeExceptionResult { get; set; }
    }
}
