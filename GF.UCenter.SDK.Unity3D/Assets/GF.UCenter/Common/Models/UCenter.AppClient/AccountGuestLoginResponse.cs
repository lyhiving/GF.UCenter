using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AccountGuestLoginResponse
    {
        [DataMember]
        public string AccountId { get; set; }
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
