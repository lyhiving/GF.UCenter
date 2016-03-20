using Newtonsoft.Json.Linq;

namespace UCenter.Common.Portable
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

    //[DataContract]
    public class UCenterResponse
    {
        //[DataMember(Name = "status")]
        public UCenterResponseStatus status { get; set; }

        //[DataMember(Name = "result")]
        public virtual JToken result { get; set; }

        //[DataMember(Name = "error")]
        public UCenterError error { get; set; }

        public T As<T>()
        {
            if (this.result == null)
            {
                return default(T);
            }

            return this.result.ToObject<T>();
        }
    }
}
