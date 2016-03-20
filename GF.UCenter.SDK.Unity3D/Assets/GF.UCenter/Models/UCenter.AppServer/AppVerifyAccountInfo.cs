using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    [Serializable]
    public class AppVerifyAccountInfo
    {
        public string AppId;
        public string AppSecret;
        public string AccountId;
        public string AccountToken;
    }
}
