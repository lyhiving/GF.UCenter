using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UCenter.Common.Portable
{
    [Serializable]
    [DataContract]
    public class AppVerifyAccountResponse
    {
        [DataMember]
        public string AccountId;
        [DataMember]
        public string AccountName;
        [DataMember]
        public string AccountToken;
        [DataMember]
        public DateTime LastLoginDateTime;
        [DataMember]
        public DateTime LastVerifyDateTime;
    }
}
