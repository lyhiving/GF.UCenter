using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
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
