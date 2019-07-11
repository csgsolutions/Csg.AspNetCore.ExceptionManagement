using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Csg.AspNetCore.ExceptionManagement
{
    public delegate Task ExceptionHandlerDelegate(ExceptionContext context);
    public delegate void ExceptionFilterDelegate(ExceptionContext context);

    public class ExceptionContext
    {
        public Exception Error { get; set; }

        public string ErrorID { get; set; }

        public HttpContext HttpContext { get; set; }

        public ExceptionResult Result { get; set; }

        public ExceptionManagementOptions Options { get; set; }
    }
}
