using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UCenter.Common.Portable
{
    [DataContract]
    public class AccountResetPasswordInfo
    {
        [DataMember]
        public string AccountId { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string SuperPassword { get; set; }
    }
}
