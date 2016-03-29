using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class UCenterResponse<TResult> : UCenterResponse
    {
        [DataMember]
        [JsonProperty("result")]
        public virtual TResult Result { get; set; }
    }

    [DataContract]
    public class UCenterResponse
    {
        [DataMember]
        [JsonProperty("status")]
        public UCenterResponseStatus Status { get; set; }

        [DataMember]
        [JsonProperty("error")]
        public UCenterError Error { get; set; }
    }
}
