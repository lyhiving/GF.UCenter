using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AppVerifyAccountRequestInfo : AppVerifyAccountInfo
    {
        [DataMember]
        public override string AppId { get; set; }
        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public override string AppSecret { get; set; }
        [DataMember]
        public override string AccountId { get; set; }
        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public override string AccountToken { get; set; }
    }
}
