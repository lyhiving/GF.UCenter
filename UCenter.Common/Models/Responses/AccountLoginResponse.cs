using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCenter.Common.Database.Entities;
using UCenter.Common.Models;

namespace UCenter.Common.Models
{
    public class AccountLoginResponse : AccountRequestResponse
    {
        public string Token { get; set; }

        public DateTime LastLoginDateTime { get; set; }

        public override void ApplyEntity(AccountEntity account)
        {
            this.Token = account.Token;
            this.LastLoginDateTime = account.LastLoginDateTime;
            base.ApplyEntity(account);
        }
    }
}