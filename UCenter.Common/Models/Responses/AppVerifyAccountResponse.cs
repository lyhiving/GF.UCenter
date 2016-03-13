using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AppVerifyAccountResponse
    {
        public string AccountId;
        public string AccountName;
        public string AccountToken;
        public DateTime LastLoginDateTime;
        public DateTime LastVerifyDateTime;
    }
}
