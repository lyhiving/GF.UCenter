namespace GF.UCenter.Common.Models.AppServer
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AppAccountDataResponse
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AppId { get; set; }

        [DataMember]
        public string Data { get; set; }
    }
}