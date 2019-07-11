using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csg.AspNetCore.ExceptionManagement.UnitTests
{
    [TestClass]
    public class UnsafeFilterTests
    {
        [TestMethod]
        public void UnsafeFilter_ReturnsGenericResponseWhenUnsafe()
        {
            var context = new ExceptionContext();

            context.Options = new ExceptionManagementOptions()
            {
                UnsafeResult = ExceptionManagementOptions.GenericErrorResult
            };

            context.Result = new ExceptionResult()
            {
                IsSafe = false
            };

            Filters.UnsafeExceptionFilter(context);

            Assert.AreEqual(context.Options.UnsafeResult, context.Result);
        }

        [TestMethod]
        public void UnsafeFilter_DoesNothinghenSafe()
        {
            var context = new ExceptionContext();

            context.Options = new ExceptionManagementOptions()
            {
                UnsafeResult = ExceptionManagementOptions.GenericErrorResult
            };

            var expectedResult = context.Result = new ExceptionResult()
            {
                IsSafe = true
            };

            Filters.UnsafeExceptionFilter(context);

            Assert.AreEqual(expectedResult, context.Result);
        }
    }
}
