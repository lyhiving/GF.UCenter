using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.UCenter.Common.Portable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GF.UCenter.Test
{
    public class UCExceptionExpectedAttribute : ExpectedExceptionBaseAttribute
    {
        public UCenterErrorCode[] ExpectedCodes { get; private set; }

        public UCExceptionExpectedAttribute(params UCenterErrorCode[] expectedCodes)
            : base()
        {
            this.ExpectedCodes = expectedCodes;
        }

        protected override void Verify(Exception exception)
        {
            if (exception == null)
            {
                Assert.Fail($"Expect {typeof(UCenterException)} exception, but no exception happened.");
            }

            if (!(exception is UCenterException))
            {
                Assert.Fail($"Expect (or subof) {typeof(UCenterException)} exception, but find {exception.GetType()}.");
            }

            var ex = exception as UCenterException;
            if (this.ExpectedCodes != null && !ExpectedCodes.Any(c => c == ex.ErrorCode))
            {
                Assert.Fail($"Expect ErrorCode: {string.Join(",", this.ExpectedCodes.Select(e => e.ToString()))}, but actual: {ex.ErrorCode}");
            }
        }
    }
}
