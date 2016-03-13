using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Attributes;
using UCenter.Common.Models;

namespace UCenter.Common.Database.Entities
{
    [DocumentType("LoginRecord")]
    public class LoginRecordEntity : BaseEntity<LoginRecordEntity>
    {
        public string AccountName { get; set; }

        public DateTime LoginTime { get; set; }

        public string ClientIp { get; set; }

        public string UserAgent { get; set; }

        public UCenterResult Code { get; set; }

        public string Comments { get; set; }
    }
}
