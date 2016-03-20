using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
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
