using System;
using System.Text;
using System.Threading.Tasks;

namespace Csg.AspNetCore.ExceptionManagement
{

    
    public class ExceptionResult
    {
        public int StatusCode { get; set; } = 500;

        public string ErrorTitle { get; set; }

        public string ErrorDetail { get; set; }

        public bool IsSafe { get; set; } = false;

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
