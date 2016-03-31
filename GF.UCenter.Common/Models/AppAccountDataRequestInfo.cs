using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AppAccountDataRequestInfo : AppAccountDataInfo
    {
        [DataMember]
        public override string AccountId { get; set; }
        [DataMember]
        public override string AppId { get; set; }
        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public override string AppSecret { get; set; }
        [DataMember]
        public override string Data { get; set; }
    }
}
