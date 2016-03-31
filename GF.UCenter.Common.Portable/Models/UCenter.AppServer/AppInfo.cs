using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AppInfo
    {
        [DataMember]
        public virtual string AppId { get; set; }
        [DataMember]
        public virtual string AppSecret { get; set; }
    }
}
