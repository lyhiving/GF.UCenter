using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AccountLoginInfo
    {
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
