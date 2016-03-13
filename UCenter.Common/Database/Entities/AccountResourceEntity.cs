using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Attributes;
using UCenter.Common.Models;

namespace UCenter.Common.Database.Entities
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
                    this.Id = $"{pointerType}-{account.AccountName}";
                    break;
                case AccountResourceType.Phone:
                    this.Id = $"{pointerType}-{account.PhoneNum}";
                    break;
                case AccountResourceType.Email:
                    this.Id = $"{pointerType}-{account.Email}";
                    break;
                default:
                    break;
            }

            this.PointerType = pointerType;
            this.AccountId = account.Id;
        }

        public string AccountId { get; set; }

        public AccountResourceType PointerType { get; set; }
    }
}
