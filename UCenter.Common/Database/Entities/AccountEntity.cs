using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Models;

namespace UCenter.Common.Database.Entities
{
    //[Serializable]
    //[DataContract]
    [DatabaseTableName("account")]
    public class AccountEntity : BaseEntity<AccountEntity>
    {
        [Column(AutoIncrement = true, IsKey = true)]
        public long AccountId { get; set; }

        [Column(Length = 32)]
        public string AccountName { get; set; }

        [Column(Length = 512)]
        [IgnoreDataMember]
        public string Password { get; set; }

        [Column(Length = 512)]
        public string Token { get; set; }

        [Column(ColumnName = "LastLoginDt")]
        public DateTime LastLoginDateTime { get; set; }

        [Column(Length = 32)]
        public string Name { get; set; }

        public Sex Sex { get; set; }

        [Column(Length = 512)]
        public string IdentityNum { get; set; }

        [Column(Length = 32)]
        public string PhoneNum { get; set; }
    }
}
