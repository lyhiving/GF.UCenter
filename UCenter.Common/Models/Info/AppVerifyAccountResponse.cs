using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AccountAppInfo
    {
        public string AppId;
        public string AccountId;
        public string AccountName;
        public string Token;
        public DateTime LastLoginDateTime;
        public AppData AppData;
    }
}
