namespace GF.UCenter.CouchBase.Entities
{
    using Attributes;
    using Common.Portable.Models.AppClient;

    [DocumentType("AccountResource")]
    public class AccountResourceEntity : BaseEntity<AccountResourceEntity>
    {
        public AccountResourceEntity()
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