using System;

namespace GF.UCenter.CouchBase
{
    [Serializable]
    [DocumentType("AppData")]
    public class AppDataEntity : BaseEntity<AppDataEntity>
    {
        public string AppId;
        public string AccountId;
        public string Data;
    }
}
