using System.Collections.Generic;
using Newtonsoft.Json;

namespace UCenter.Common.Models
{
    public class Charge
    {
        [JsonProperty("amount")]
        public int? Amount { get; set; }
        [JsonProperty("amount_refunded")]
        public int? Amount_refunded { get; set; }
        [JsonProperty("amount_settle")]
        public int? Amount_settle { get; set; }
        [JsonProperty("app")]
        public string App { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("client_ip")]
        public string Client_ip { get; set; }
        [JsonProperty("created")]
        public int? Created { get; set; }
        [JsonProperty("credential")]
        public object Credential { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("extra")]
        public Dictionary<string, object> Extra { get; set; }
        [JsonProperty("failure_code")]
        public string Failure_code { get; set; }
        [JsonProperty("failure_msg")]
        public string Failure_msg { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("livemode")]
        public bool Livemode { get; set; }
        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("order_no")]
        public string Order_no { get; set; }
        [JsonProperty("paid")]
        public bool Paid { get; set; }
        [JsonProperty("refunded")]
        public bool Refunded { get; set; }
        [JsonProperty("refunds")]
        public object Refunds { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("time_expire")]
        public int? Time_expire { get; set; }
        [JsonProperty("time_paid")]
        public int? Time_paid { get; set; }
        [JsonProperty("time_settle")]
        public int? Time_settle { get; set; }
        [JsonProperty("transaction_no")]
        public string Transaction_no { get; set; }
    }
}