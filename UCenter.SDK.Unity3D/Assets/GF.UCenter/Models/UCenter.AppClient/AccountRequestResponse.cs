using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    public class AccountRequestResponse
    {
        public string AccountName { get; set; }

        public string Name { get; set; }

        public Sex Sex { get; set; }

        public string IdentityNum { get; set; }

        public string PhoneNum { get; set; }

        public virtual void ApplyEntity(AccountResponse account)
        {
            this.AccountName = account.AccountName;
            this.Name = account.Name;
            this.Sex = account.Sex;
            this.IdentityNum = account.IdentityNum;
            this.PhoneNum = account.PhoneNum;
        }
    }
}