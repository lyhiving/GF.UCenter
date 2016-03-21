using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace UCenter.Common.Portable
{
    [DataContract]
    public class Charge
    {
        [DataMember]
        [JsonProperty("amount")]
        public int? Amount { get; set; }
        [DataMember]
        [JsonProperty("amount_refunded")]
        public int? Amount_refunded { get; set; }
        [DataMember]
        [JsonProperty("amount_settle")]
        public int? Amount_settle { get; set; }
        [DataMember]
        [JsonProperty("app")]
        public string App { get; set; }
        [DataMember]
        [JsonProperty("body")]
        public string Body { get; set; }
        [DataMember]
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [DataMember]
        [JsonProperty("client_ip")]
        public string Client_ip { get; set; }
        [DataMember]
        [JsonProperty("created")]
        public int? Created { get; set; }
        [DataMember]
        [JsonProperty("credential")]
        public object Credential { get; set; }
        [DataMember]
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [DataMember]
        [JsonProperty("description")]
        public string Description { get; set; }
        [DataMember]
        [JsonProperty("extra")]
        public Dictionary<string, object> Extra { get; set; }
        [DataMember]
        [JsonProperty("failure_code")]
        public string Failure_code { get; set; }
        [DataMember]
        [JsonProperty("failure_msg")]
        public string Failure_msg { get; set; }
        [DataMember]
        [JsonProperty("id")]
        public string Id { get; set; }
        [DataMember]
        [JsonProperty("livemode")]
        public bool Livemode { get; set; }
        [DataMember]
        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
        [DataMember]
        [JsonProperty("object")]
        public string Object { get; set; }
        [DataMember]
        [JsonProperty("order_no")]
        public string Order_no { get; set; }
        [DataMember]
        [JsonProperty("paid")]
        public bool Paid { get; set; }
        [DataMember]
        [JsonProperty("refunded")]
        public bool Refunded { get; set; }
        [DataMember]
        [JsonProperty("refunds")]
        public object Refunds { get; set; }
        [DataMember]
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [DataMember]
        [JsonProperty("time_expire")]
        public int? Time_expire { get; set; }
        [DataMember]
        [JsonProperty("time_paid")]
        public int? Time_paid { get; set; }
        [DataMember]
        [JsonProperty("time_settle")]
        public int? Time_settle { get; set; }
        [DataMember]
        [JsonProperty("transaction_no")]
        public string Transaction_no { get; set; }
    }
}