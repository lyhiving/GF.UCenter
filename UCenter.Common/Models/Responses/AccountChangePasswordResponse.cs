using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCenter.Common.Database.Entities;
using UCenter.Common.Models;

namespace UCenter.Common.Models
{
    public class AccountChangePasswordResponse : AccountRequestResponse
    {
        public string Token { get; private set; }

        public DateTime LastLoginDateTime { get; private set; }

        public override void ApplyEntity(AccountEntity account)
        {
            this.Token = account.Token;
            this.LastLoginDateTime = account.LastLoginDateTime;
            base.ApplyEntity(account);
        }
    }
}