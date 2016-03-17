using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace UCenter.Common.SDK
{
    public class UCenterResponse<T> : UCenterResponse
    {
        public T Content
        {
            get
            {
                return base.As<T>();
            }
        }
    }

    [DataContract]
    public class UCenterResponse
    {
        [DataMember(Name = "status")]
        public UCenterResponseStatus Status { get; set; }

        [DataMember(Name = "result")]
        public virtual JToken JsonResult { get; set; }

        [DataMember(Name = "error")]
        public UCenterError Error { get; set; }

        public T As<T>()
        {
            if (this.JsonResult == null)
            {
                return default(T);
            }

            return this.JsonResult.ToObject<T>();
        }
    }
}
