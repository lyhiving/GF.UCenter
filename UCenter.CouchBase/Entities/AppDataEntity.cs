using System;
using UCenter.CouchBase.Attributes;

namespace UCenter.CouchBase.Entities
{
    [Serializable]
    [DocumentType("AppData")]
    public class AppDataEntity : BaseEntity<AppDataEntity>
    {
        public string AppId;
        public string AccountName;
        public string Data;
    }
}
