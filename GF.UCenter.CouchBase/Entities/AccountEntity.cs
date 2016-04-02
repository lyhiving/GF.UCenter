using System;
using GF.UCenter.Common.Portable;

namespace GF.UCenter.CouchBase
{
    [DocumentType("Account")]
    public class AccountEntity : BaseEntity<AccountEntity>
    {
        public string AccountName { get; set; }

        public string Password { get; set; }

        public string SuperPassword { get; set; }

        public string Token { get; set; }

        public DateTime LastLoginDateTime { get; set; }

        public bool IsGuest { get; set; }

        public string Name { get; set; }

        public string ProfileImage { get; set; }

        public string ProfileThumbnail { get; set; }

        public Sex Sex { get; set; }

        public string IdentityNum { get; set; }

        public string PhoneNum { get; set; }

        public string Email { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
