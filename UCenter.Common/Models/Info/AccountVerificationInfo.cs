using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AccountVerificationInfo
    {
        public string AppId;
        public string AppSecret;
        public string AccountName;
        public string AccountToken;
    }
}
