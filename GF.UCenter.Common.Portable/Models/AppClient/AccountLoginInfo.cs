namespace GF.UCenter.Common.Portable
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AccountLoginInfo
    {
        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}