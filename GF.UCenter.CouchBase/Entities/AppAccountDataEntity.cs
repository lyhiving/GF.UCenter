namespace GF.UCenter.CouchBase
{
    using System;

    [Serializable]
    [DocumentType("AppAccountData")]
    public class AppAccountDataEntity : BaseEntity<AppAccountDataEntity>
    {
        public string AppId { get; set; }

        public string AccountId { get; set; }

        public string Data { get; set; }
    }
}