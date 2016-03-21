using System.Runtime.Serialization;

namespace UCenter.Common.Portable
{
    [DataContract]
    public class AppResponse
    {
        [DataMember]
        public string AppId { get; set; }
        [DataMember]
        public string AppSecret { get; set; }
        [DataMember]
        public string Token { get; set; }
    }
}
