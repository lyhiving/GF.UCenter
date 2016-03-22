using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AccountRequestResponse
    {
        [DataMember]
        public string AccountId { get; set; }
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Sex Sex { get; set; }
        [DataMember]
        public string IdentityNum { get; set; }
        [DataMember]
        public string PhoneNum { get; set; }

        public virtual void ApplyEntity(AccountResponse account)
        {
            this.AccountId = account.AccountId;
            this.AccountName = account.AccountName;
            this.Name = account.Name;
            this.Sex = account.Sex;
            this.IdentityNum = account.IdentityNum;
            this.PhoneNum = account.PhoneNum;
        }
    }
}
