namespace GF.UCenter.Common.Portable
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract]
    public class UCenterError
    {
        [DataMember]
        [JsonProperty("code")]
        public UCenterErrorCode ErrorCode { get; set; }

        [DataMember]
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}