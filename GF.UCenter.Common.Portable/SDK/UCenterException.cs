using System;

namespace UCenter.Common.Portable
{
    public class UCenterException : ApplicationException
    {
        public UCenterErrorCode ErrorCode { get; private set; }

        public UCenterException(UCenterErrorCode errorCode, string message)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }
    }
}