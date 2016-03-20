using System;
using System.Collections.Generic;

    public class AccountLoginResponse : AccountRequestResponse
    {
        public string Token { get; set; }

        public DateTime LastLoginDateTime { get; set; }

        public override void ApplyEntity(AccountResponse account)
        {
            this.Token = account.Token;
            this.LastLoginDateTime = account.LastLoginDateTime;
            base.ApplyEntity(account);
        }
    }
