using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UCenter.Common.Portable
{
    [DataContract]
    public class AccountConvertResponse : AccountRequestResponse
    {
        [DataMember]
        public string Token { get; private set; }

        public override void ApplyEntity(AccountResponse account)
        {
            this.Token = account.Token;
            base.ApplyEntity(account);
        }
    }
}