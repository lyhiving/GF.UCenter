using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AccountResetPasswordInfo
    {
        [DataMember]
        public string AccountId { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string SuperPassword { get; set; }
    }
}
