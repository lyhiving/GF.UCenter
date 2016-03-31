using System.Runtime.Serialization;
using GF.UCenter.Common.Portable;

namespace GF.UCenter.Common.Models
{
    public class AppRequestInfo : AppInfo
    {
        [DataMember]
        public override string AppId { get; set; }
        [DataMember]
        [DumperTo("<--AppSecret-->")]
        public override string AppSecret { get; set; }
    }
}