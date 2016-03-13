using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AppDataEntity : BaseEntity<AppDataEntity>
    {
        public string AccountId;
        public string AppId;
        public string Data;
    }
}
