using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace UCenter.SDK
{
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
