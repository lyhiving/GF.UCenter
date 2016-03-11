using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Models;

namespace UCenter.Common.Database.Entities
{
    [DatabaseTableName("account")]
    public class LoginRecordEntity : BaseEntity<LoginRecordEntity>
    {
        [Column(Length = 32)]
        public string AccountName { get; set; }

        public DateTime LoginTime { get; set; }

        [Column(Length = 32, Nullable = false)]
        public string ClientIp { get; set; }

        [Column(Length = 512)]
        public string UserAgent { get; set; }

        public UCenterResult Code { get; set; }

        [Column(Length = 1024)]
        public string Comments { get; set; }
    }
}
