namespace GF.UCenter.Common
{
    using System.Runtime.Serialization;

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