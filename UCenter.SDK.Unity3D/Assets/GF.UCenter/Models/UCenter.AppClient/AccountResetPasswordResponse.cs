using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    public class AccountResetPasswordResponse : AccountRequestResponse
    {
        public string Token { get; private set; }

        public DateTime LastLoginDateTime { get; private set; }

        public override void ApplyEntity(AccountResponse account)
        {
            this.Token = account.Token;
            this.LastLoginDateTime = account.LastLoginDateTime;
            base.ApplyEntity(account);
        }
    }
}