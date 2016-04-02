namespace GF.UCenter.Common.Models.PingPlusPlus
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class RefundList
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("data")]
        public IEnumerable<Refund> Data { get; set; }
    }
}