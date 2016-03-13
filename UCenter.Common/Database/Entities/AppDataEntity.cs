using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Attributes;
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Models
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
