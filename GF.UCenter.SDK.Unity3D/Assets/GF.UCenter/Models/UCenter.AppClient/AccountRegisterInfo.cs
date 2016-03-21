using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UCenter.Common.Portable
{
    [DataContract]
    public class AccountRegisterInfo
    {
        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string SuperPassword { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PhoneNum { get; set; }

        [DataMember]
        public string IdentityNum { get; set; }

        [DataMember]
        public Sex Sex { get; set; }
    }
}
