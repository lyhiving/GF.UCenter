using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AppDataResponse
    {
        public string AppId;
        public string AccountName;
        public string Data;
    }
}
