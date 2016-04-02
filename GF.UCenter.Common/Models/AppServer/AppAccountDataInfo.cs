namespace GF.UCenter.Common
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AppAccountDataInfo
    {
        [DataMember]
        public virtual string AccountId { get; set; }

        [DataMember]
        public virtual string AppId { get; set; }

        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public virtual string AppSecret { get; set; }

        [DataMember]
        public virtual string Data { get; set; }
    }
}