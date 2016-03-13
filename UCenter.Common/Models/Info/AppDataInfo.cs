using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AppDataInfo
    {
        public string AppId;
        public string AppSecret;
        public string AccountId;
        public string Data;
    }
}
