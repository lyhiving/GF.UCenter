using System;

namespace GF.UCenter.CouchBase
{
    [Serializable]
    [DocumentType("AppAccountData")]
    public class AppAccountDataEntity : BaseEntity<AppAccountDataEntity>
    {
        public string AppId;
        public string AccountId;
        public string Data;
    }
}
