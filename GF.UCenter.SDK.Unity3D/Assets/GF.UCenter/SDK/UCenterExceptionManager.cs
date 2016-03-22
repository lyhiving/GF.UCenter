namespace GF.UCenter.Common.Portable
{
    public static class UCenterExceptionManager
    {
        public static UCenterException FromError(UCenterError error)
        {
            return new UCenterException(error.ErrorCode, error.Message);
        }
    }
}