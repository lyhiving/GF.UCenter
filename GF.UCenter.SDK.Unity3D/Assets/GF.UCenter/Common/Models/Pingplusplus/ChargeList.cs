using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace GF.UCenter.Common.Portable
{
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