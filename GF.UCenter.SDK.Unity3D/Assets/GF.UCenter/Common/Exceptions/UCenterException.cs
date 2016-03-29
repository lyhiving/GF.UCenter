using System;

namespace GF.UCenter.Common.Portable
{
    public class UCenterException : ApplicationException
    {
        public UCenterErrorCode ErrorCode { get; private set; }

        public UCenterException(UCenterErrorCode errorCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        public UCenterException(UCenterErrorCode errorCode, Exception innerException = null)
            : this(errorCode, UCenterResourceManager.GetErrorMessage(errorCode), innerException)
        {
        }
    }
}