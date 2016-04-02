namespace GF.UCenter.CouchBase
{
    using System;
    using Common.Portable;

    [DocumentType("LoginRecord")]
    public class LoginRecordEntity : BaseEntity<LoginRecordEntity>
    {
        public string AccountName { get; set; }

        public string AccountId { get; set; }

        public DateTime LoginTime { get; set; }

        public string ClientIp { get; set; }

        public string UserAgent { get; set; }

        public UCenterErrorCode Code { get; set; }

        public string Comments { get; set; }
    }
}