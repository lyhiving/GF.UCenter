using Newtonsoft.Json;

namespace UCenter.Common.Portable
{
    public class UCenterError
    {
        [JsonProperty("code")]
        public UCenterErrorCode ErrorCode { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
