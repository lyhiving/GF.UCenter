using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AppInfo
    {
        [DataMember]
        public string AppId { get; set; }
        [DataMember]
        public string AppSecret { get; set; }
    }
}
