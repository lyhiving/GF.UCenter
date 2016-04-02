namespace GF.UCenter.CouchBase.Entities
{
    using System;
    using Attributes;

    [Serializable]
    [DocumentType("AppAccountData")]
    public class AppAccountDataEntity : BaseEntity<AppAccountDataEntity>
    {
        public string AppId { get; set; }

        public string AccountId { get; set; }

        public string Data { get; set; }
    }
}