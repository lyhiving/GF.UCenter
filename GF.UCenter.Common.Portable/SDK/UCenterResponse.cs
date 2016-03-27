using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GF.UCenter.Common.Portable
{
    //public class UCenterResponse<T> : UCenterResponse
    //{
    //    public T Content
    //    {
    //        get { return base.As<T>(); }
    //    }
    //}

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
