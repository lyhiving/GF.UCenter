using System;
using UCenter.Common.Models;
using UCenter.CouchBase.Attributes;

namespace UCenter.CouchBase.Entities
{
    [DocumentType("LoginRecord")]
    public class LoginRecordEntity : BaseEntity<LoginRecordEntity>
    {
        public string AccountName { get; set; }

        public DateTime LoginTime { get; set; }

        public string ClientIp { get; set; }

        public string UserAgent { get; set; }

        public UCenterErrorCode Code { get; set; }

        public string Comments { get; set; }
    }
}
