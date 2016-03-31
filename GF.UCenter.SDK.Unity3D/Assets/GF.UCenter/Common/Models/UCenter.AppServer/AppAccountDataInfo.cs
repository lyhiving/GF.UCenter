using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AppAccountDataInfo
    {
        [DataMember]
        public virtual string AccountId { get; set; }
        [DataMember]
        public virtual string AppId { get; set; }
        [DataMember]
        public virtual string AppSecret { get; set; }
        [DataMember]
        public virtual string Data { get; set; }
    }
}
