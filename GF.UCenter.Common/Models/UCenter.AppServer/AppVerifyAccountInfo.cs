using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AppVerifyAccountInfo
    {
        [DataMember]
        public virtual string AppId { get; set; }
        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public virtual string AppSecret { get; set; }
        [DataMember]
        public virtual string AccountId { get; set; }
        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public virtual string AccountToken { get; set; }
    }
}
