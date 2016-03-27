using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GF.UCenter.Common.Portable
{
    internal static class UCenterResourceManager
    {
        private static readonly Dictionary<UCenterErrorCode, string> errorMessages = new Dictionary<UCenterErrorCode, string>();
        private const string GeneralErrorMessage = "Internal server error.";
        static UCenterResourceManager()
        {
            errorMessages.Add(UCenterErrorCode.AccountLoginFailedPasswordNotMatch, @"Account not exists or password is wrong.");
            errorMessages.Add(UCenterErrorCode.AccountLoginFailedTokenNotMatch, @"Account not exists or password is wrong.");
            errorMessages.Add(UCenterErrorCode.AccountNotExist, @"Account not exists.");
            errorMessages.Add(UCenterErrorCode.AccountRegisterFailedAlreadyExist, @"Account already exists.");
            errorMessages.Add(UCenterErrorCode.AppAuthFailedSecretNotMatch, @"App not exists or password is wrong.");
            errorMessages.Add(UCenterErrorCode.AppNotExit, @"App not exists.");
            errorMessages.Add(UCenterErrorCode.AppReadAccountDataFailed, @"Read data failed.");
            errorMessages.Add(UCenterErrorCode.AppWriteAccountDataFailed, @"Write data failed.");
            errorMessages.Add(UCenterErrorCode.CouchBaseError, @"Internal database error.");
            errorMessages.Add(UCenterErrorCode.Failed, @"Request execution failed.");
            errorMessages.Add(UCenterErrorCode.HttpClientError, @"Client error.");
            errorMessages.Add(UCenterErrorCode.PaymentCreateChargeFailed, @"Payment create failed.");
            errorMessages.Add(UCenterErrorCode.Success, @"Request accepted");
        }

        public static string GetErrorMessage(UCenterErrorCode errorCode)
        {
            if (errorMessages.ContainsKey(errorCode))
            {
                return errorMessages[errorCode];
            }
            else
            {
                return GeneralErrorMessage;
            }
        }
    }
}
