using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AppVerifyAccountResponse
    {
        [DataMember]
        public string AccountId { get; set; }
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public string AccountToken { get; set; }
        [DataMember]
        public DateTime LastLoginDateTime { get; set; }
        [DataMember]
        public DateTime LastVerifyDateTime { get; set; }
    }
}
