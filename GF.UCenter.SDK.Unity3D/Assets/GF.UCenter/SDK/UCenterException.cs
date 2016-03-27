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
            : this(errorCode, GenerateErrorMessage(errorCode), innerException)
        {
        }

        private static string GenerateErrorMessage(UCenterErrorCode errorCode)
        {
            try
            {
                return UCResource.ResourceManager.GetString("Msg_" + errorCode.ToString());
            }
            catch (Exception)
            {
                return UCResource.ResourceManager.GetString("Msg_General");
            }
        }
    }
}