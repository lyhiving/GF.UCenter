using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
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
