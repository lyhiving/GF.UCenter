using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    public class AccountRegisterResponse : AccountRequestResponse
    {
        public string Token { get; private set; }

        public override void ApplyEntity(AccountResponse account)
        {
            this.Token = account.Token;
            base.ApplyEntity(account);
        }
    }
}