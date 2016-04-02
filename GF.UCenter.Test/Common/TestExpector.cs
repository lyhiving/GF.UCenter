namespace GF.UCenter.Test.Common
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using UCenter.Common.Portable.Contracts;
    using UCenter.Common.Portable.Exceptions;

    public static class TestExpector
    {
        public static void ExpectException<TExpection>(Action action)
            where TExpection : Exception
        {
            try
            {
                action();
                Assert.Fail($"Expect {typeof (TExpection)} exception, but no expection happened");
            }
            catch (TExpection)
            {
                // Pass 
            }
        }

        public static void ExpectUCenterError(UCenterErrorCode expectedErrorCode, Action action)
        {
            try
            {
                action();
                Assert.Fail($"Expected {typeof (UCenterException)} Exception, but no expection happened");
            }
            catch (UCenterException ex)
            {
                if (ex.ErrorCode != expectedErrorCode)
                {
                    Assert.Fail($"Expect ErrorCode: {expectedErrorCode} but actual: {ex.ErrorCode}");
                }
            }
            catch (AggregateException ex)
            {
                UCenterException actual = null;
                if (ex.InnerExceptions.Count() == 1)
                {
                    actual = ex.InnerExceptions.Single() as UCenterException;
                }
                if (actual == null)
                {
                    throw;
                }
                if (actual.ErrorCode != expectedErrorCode)
                {
                    Assert.Fail($"Expect ErrorCode: {expectedErrorCode} but actual: {actual.ErrorCode}");
                }
            }
        }

        public static async void ExpectUCenterErrorAsync(UCenterErrorCode expectedErrorCode, Func<Task> func)
        {
            try
            {
                await func();
                Assert.Fail("Expected {typeof(UCenterException)}, but no expection happened");
            }
            catch (UCenterException ex)
            {
                if (ex.ErrorCode != expectedErrorCode)
                {
                    Assert.Fail($"Expect ErrorCode: {expectedErrorCode} but actual: {ex.ErrorCode}");
                }
            }
        }
    }
}