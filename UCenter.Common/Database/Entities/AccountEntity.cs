using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Models;

namespace UCenter.Common.Database.Entities
{
    [DatabaseTableName("account")]
    public class AccountEntity : BaseEntity
    {
        [Column(AutoIncrement = true, IsKey = true)]
        public long AccountId { get; set; }

        public string AccountName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        // todo: get back the datetime type when resolve insert format error.
        [Column(ColumnName = "LastLoginDt", Ignore = true)]
        public DateTime LastLoginDateTime { get; set; }

        public string Name { get; set; }

        public Sex Sex { get; set; }

        public string IdentityNum { get; set; }

        public string PhoneNum { get; set; }
    }
}
