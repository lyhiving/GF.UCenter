using GF.UCenter.Common.Portable;

namespace GF.UCenter.CouchBase
{
    [DocumentType("AccountResource")]
    public class AccountResourceEntity : BaseEntity<AccountResourceEntity>
    {
        public AccountResourceEntity()
            : base()
        {
        }

        public AccountResourceEntity(AccountEntity account, AccountResourceType pointerType)
        {
            switch (pointerType)
            {
                case AccountResourceType.None:
                    break;
                case AccountResourceType.AccountName:
                    this.Id = GenerateResourceId(AccountResourceType.AccountName, account.AccountName);
                    break;
                case AccountResourceType.Phone:
                    this.Id = GenerateResourceId(AccountResourceType.Phone, account.PhoneNum);
                    break;
                case AccountResourceType.Email:
                    this.Id = GenerateResourceId(AccountResourceType.Email, account.Email);
                    break;
                default:
                    break;
            }

            this.PointerType = pointerType;
            this.AccountId = account.Id;
        }

        public string AccountId { get; set; }

        public AccountResourceType PointerType { get; set; }

        public static string GenerateResourceId(AccountResourceType resourceType, string postfixId)
        {
            return $"{resourceType}-{postfixId}";
        }
    }
}
