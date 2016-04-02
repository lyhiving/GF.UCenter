namespace GF.UCenter.Common.Models.PingPlusPlus
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract]
    public class ChargeList
    {
        [DataMember]
        [JsonProperty("data")]
        public IEnumerable<Charge> Data { get; set; }

        [DataMember]
        [JsonProperty("has_more")]
        public bool Has_more { get; set; }

        [DataMember]
        [JsonProperty("object")]
        public string Object { get; set; }

        [DataMember]
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}