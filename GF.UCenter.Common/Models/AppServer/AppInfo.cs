namespace GF.UCenter.Common.Models.AppServer
{
    using System.Runtime.Serialization;
    using Dumper;

    [DataContract]
    public class AppInfo
    {
        [DataMember]
        public virtual string AppId { get; set; }

        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public virtual string AppSecret { get; set; }
    }
}