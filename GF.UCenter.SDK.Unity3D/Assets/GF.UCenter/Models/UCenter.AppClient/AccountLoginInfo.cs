using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UCenter.Common.Portable
{
    [Serializable]
    [DataContract]
    public class AccountLoginInfo
    {
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
