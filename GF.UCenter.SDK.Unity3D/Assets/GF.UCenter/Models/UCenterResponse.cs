using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UCenter.Common.Portable
{
    public class UCenterResponse<T> : UCenterResponse
    {
        public T Content
        {
            get { return base.As<T>(); }
        }
    }

    [DataContract]
    public class UCenterResponse
    {
        [DataMember]
        [JsonProperty("status")]
        public UCenterResponseStatus status { get; set; }
        [DataMember]
        [JsonProperty("result")]
        public virtual JToken result { get; set; }
        [DataMember]
        [JsonProperty("error")]
        public UCenterError error { get; set; }

        public T As<T>()
        {
            if (this.result == null) return default(T);
            else return this.result.ToObject<T>();
        }
    }
}
