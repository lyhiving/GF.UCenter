using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AccountAppVerificationInfo
    {
        public string AppId;
        public string AppSecret;
        public string AccountId;
        public string AccountName;
        public string AccountToken;
        public bool GetAppData;
    }
}
