using System.Collections.Generic;
using Newtonsoft.Json;

namespace UCenter.Common.Portable
{
    public class ChargeList
    {
        [JsonProperty("data")]
        public IEnumerable<Charge> Data { get; set; }
        [JsonProperty("has_more")]
        public bool Has_more { get; set; }
        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}